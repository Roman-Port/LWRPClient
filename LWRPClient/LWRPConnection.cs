using LWRPClient.Entities;
using LWRPClient.Exceptions;
using LWRPClient.Layer1.Transports;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;
using LWRPClient.Core;
using LWRPClient.Features;
using LWRPClient.Core.Sources;
using LWRPClient.Core.Destinations;

namespace LWRPClient
{
    public class LWRPConnection : IDisposable
    {
        public delegate void InfoDataReceivedEventArgs(LWRPConnection conn);
        public delegate void ConnectionStateUpdateEventArgs(LWRPConnection conn, LWRPState state);
        public delegate void GroupProcessingBeginEventArgs(LWRPConnection conn);
        public delegate void GroupProcessingEndEventArgs(LWRPConnection conn);
        public delegate void MessageSubscriptionEvent(LWRPMessage message, bool inGroup);
        public delegate void BatchUpdateEventArgs<T>(LWRPConnection conn, T[] updates);

        public LWRPConnection(ILWRPTransport transport, LWRPEnabledFeature enabledFeatures)
        {
            //Set transport and map events
            this.transport = transport;
            transport.OnConnected += Transport_OnConnected;
            transport.OnDisconnected += Transport_OnDisconnected;
            transport.OnMessageReceived += Transport_OnMessageReceived;

            //Create features
            this.enabledFeatures = enabledFeatures;
            if ((enabledFeatures & LWRPEnabledFeature.SOURCES) == LWRPEnabledFeature.SOURCES)
                features.Add(new SourcesFeature(this));
            if ((enabledFeatures & LWRPEnabledFeature.DESTINATIONS) == LWRPEnabledFeature.DESTINATIONS)
                features.Add(new DestinationsFeature(this));

            //Misc
            readyTask = new TaskCompletionSource<LWRPConnection>();
        }

        public LWRPConnection(IPAddress address, LWRPEnabledFeature features, int port = 93) : this(new TcpLWRPTransport(new IPEndPoint(address, port)), features)
        {

        }

        internal readonly ILWRPTransport transport;
        internal readonly object mutex = new object();
        private readonly LWRPEnabledFeature enabledFeatures;

        /// <summary>
        /// True if we've called Initialize to keep the socket connection
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// True once we send VER and get a response from the device.
        /// </summary>
        private bool hasInfoData = false;

        /// <summary>
        /// True while in groups being processed.
        /// </summary>
        private bool isProcessingGroup = false;

        /// <summary>
        /// If true, do not re-establish connections when disposed
        /// </summary>
        private bool isDisposing = false;

        /// <summary>
        /// Current state of the connection
        /// </summary>
        private LWRPState state = LWRPState.PRE_INIT;

        /// <summary>
        /// Task that is completed when all info data is recieved for the first time.
        /// </summary>
        private TaskCompletionSource<LWRPConnection> readyTask;

        /// <summary>
        /// Current message subscriptions.
        /// </summary>
        private readonly List<MessageSubscription> subscriptions = new List<MessageSubscription>();

        /// <summary>
        /// Features that are currently enabled. Will only be added to during construction.
        /// There should only be one of each type in this.
        /// </summary>
        private readonly List<BaseFeature> features = new List<BaseFeature>();

        //The following are details obtained with "VER"
        private string infoLwrpVer;
        private string infoDeviceName;
        private string infoDeviceVer;
        private string infoDeviceModel;
        private int infoSrcNum;
        private int infoDstNum;
        private int infoGpiNum;
        private int infoGpoNum;

        /* GETTERS */

        /// <summary>
        /// Current state of the connection. Thread-safe.
        /// </summary>
        public LWRPState State
        {
            get
            {
                LWRPState response;
                lock (mutex)
                    response = state;
                return response;
            } private set
            {
                lock (mutex)
                    state = value;
                OnConnectionStateUpdate?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets the features requested to be enabled.
        /// </summary>
        public LWRPEnabledFeature EnabledFeatures => enabledFeatures;

        /// <summary>
        /// True if basic information is received.
        /// </summary>
        public bool HasInfoData
        {
            get
            {
                bool result;
                lock (mutex)
                    result = hasInfoData;
                return result;
            }
        }

        /// <summary>
        /// LWRP sources on this device.
        /// </summary>
        public ILWRPSourcesFeature Sources => GetFeature<SourcesFeature>("sources");

        /// <summary>
        /// LWRP destinations on this device.
        /// </summary>
        public ILWRPDestinationsFeature Destinations => GetFeature<DestinationsFeature>("destinations");

        /// <summary>
        /// LWRP version from device. May be null. Only readable once info data has been received.
        /// </summary>
        public string LwrpVersion
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoLwrpVer;
            }
        }

        /// <summary>
        /// Device name. May be null. Only readable once info data has been received.
        /// </summary>
        public string DeviceName
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoDeviceName;
            }
        }

        /// <summary>
        /// Device version. May be null. Only readable once info data has been received.
        /// </summary>
        public string DeviceVersion
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoDeviceVer;
            }
        }

        /// <summary>
        /// Device model. May be null. Only readable once info data has been received.
        /// </summary>
        public string DeviceModel
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoDeviceModel;
            }
        }

        /// <summary>
        /// Number of sources. Only readable once info data has been received.
        /// </summary>
        public int SrcNum
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoSrcNum;
            }
        }

        /// <summary>
        /// Number of destinations. Only readable once info data has been received.
        /// </summary>
        public int DstNum
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoDstNum;
            }
        }

        /// <summary>
        /// Number of GPIs. Only readable once info data has been received.
        /// </summary>
        public int GpiNum
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoGpiNum;
            }
        }

        /// <summary>
        /// Number of GPOs. Only readable once info data has been received.
        /// </summary>
        public int GpoNum
        {
            get
            {
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();
                return infoGpoNum;
            }
        }

        /* EVENTS */

        public event InfoDataReceivedEventArgs OnInfoDataReceived;
        public event ConnectionStateUpdateEventArgs OnConnectionStateUpdate;
        public event GroupProcessingBeginEventArgs OnGroupProcessingBegin;
        public event GroupProcessingEndEventArgs OnGroupProcessingEnd;

        /// <summary>
        /// Starts to connect. Only call this once.
        /// </summary>
        public void Initialize()
        {
            //Reject if already initialized
            if (initialized)
                throw new Exception("LWRPConnection is already initialized.");
            initialized = true;

            //Reset state
            ResetState();

            //Init transport
            transport.Initialize();
        }

        /// <summary>
        /// Returns a task that can be awaited until the client is ready.
        /// </summary>
        /// <returns></returns>
        public Task WaitForReadyAsync()
        {
            return readyTask.Task;
        }

        /// <summary>
        /// Returns a task that can be awaited until the client is ready with a timeout.
        /// </summary>
        /// <returns></returns>
        public async Task WaitForReadyAsync(TimeSpan timeout)
        {
            Task timeoutTask = Task.Delay(timeout);
            Task readyTask = this.readyTask.Task;
            Task completedTask = await Task.WhenAny(timeoutTask, readyTask);
            if (completedTask != readyTask)
                throw new TimeoutException();
        }

        /// <summary>
        /// Subscribes to get events when a specific message type is recieved.
        /// </summary>
        /// <param name="name">The name of the message to target.</param>
        /// <param name="evt">The event to raise.</param>
        public void SubscribeToMessage(string name, MessageSubscriptionEvent evt)
        {
            lock (subscriptions)
                subscriptions.Add(new MessageSubscription(name, evt));
        }

        /// <summary>
        /// Unsubscribes a subscribed event. Returns true if an event was removed.
        /// </summary>
        /// <param name="name">The name of the message to target.</param>
        /// <param name="evt">The event to raise.</param>
        /// <returns></returns>
        public bool UnsubscribeFromMessage(string name, MessageSubscriptionEvent evt)
        {
            MessageSubscription[] matches;
            lock (subscriptions)
            {
                //Find matching
                matches = subscriptions.Where(x => x.Name == name.ToUpper() && x.Event == evt).ToArray();

                //Remove all
                foreach (var s in matches)
                    subscriptions.Remove(s);
            }
            return matches.Length > 0;
        }

        /// <summary>
        /// Finds a feature matching the specified type and returns it. If it wasn't found, throws a FeatureNotEnabledException.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private T GetFeature<T>(string friendlyName)
        {
            //Search
            foreach (var f in features)
            {
                if (f is T feature)
                    return feature;
            }

            //Not found
            throw new FeatureNotEnabledException(friendlyName);
        }

        /// <summary>
        /// Resets state before connecting
        /// </summary>
        private void ResetState()
        {
            //Reset state to initializing
            State = LWRPState.CONNECTING;

            //Fire event on all features
            foreach (var f in features)
                f.ResetState();
        }

        /// <summary>
        /// Submit any changes made. Returns the number of items updated.
        /// </summary>
        public async Task<int> SendUpdatesAsync()
        {
            //Process in mutex
            List<LWRPMessage> updates = new List<LWRPMessage>();
            int count;
            lock (mutex)
            {
                //Loop through all features to add updates
                foreach (var f in features)
                    f.Apply(updates);

                //Save count
                count = updates.Count;

                //If there were none, do nothing
                if (count == 0)
                    return 0;

                //If there were more than one, encapsulate it in BEGIN and END messages to process as a group
                /*if (updates.Count > 1)
                {
                    updates.Insert(0, new LWRPMessage("BEGIN", new LWRPToken[0][]));
                    updates.Add(new LWRPMessage("END", new LWRPToken[0][]));
                }*/
            }

            //Send
            await transport.SendMessages(updates.ToArray());

            return count;
        }

        /// <summary>
        /// Submit any changes made with a timeout. Returns the number of items updated.
        /// </summary>
        public async Task<int> SendUpdatesAsync(TimeSpan timeout)
        {
            Task timeoutTask = Task.Delay(timeout);
            Task<int> submitTask = SendUpdatesAsync();
            Task completedTask = await Task.WhenAny(timeoutTask, submitTask);
            if (completedTask != submitTask)
                throw new TimeoutException();
            return await submitTask;
        }

        public void Dispose()
        {
            lock (mutex)
            {
                //Set flag
                isDisposing = true;

                //Shut down transport
                transport.Dispose();

                //Set state
                State = LWRPState.DISPOSED;
            }
        }

        private void Transport_OnConnected(ILWRPTransport transport)
        {
            //Update state
            State = LWRPState.INITIALIZING;

            //Send LOGIN to make changes
            transport.SendMessage(new LWRPMessage("LOGIN", new LWRPToken[][]
            {
                new LWRPToken[] { new LWRPToken(false, "") }
            }));

            //Send VER to get info about the device
            transport.SendMessage(new LWRPMessage("VER", new LWRPToken[0][]));
        }

        private void Transport_OnDisconnected(ILWRPTransport transport, Exception ex)
        {
            if (!isDisposing)
            {
                ResetState();
            }
        }

        private void Transport_OnMessageReceived(ILWRPTransport transport, LWRPMessage message)
        {
            //Switch depending on message type
            switch (message.Name.ToUpper())
            {
                case "BEGIN": ProcessBeginGroup(); break;
                case "END": ProcessEndGroup(); break;
                case "VER": ProcessVer(message); break;
            }

            //Dispatch to subscriptions - First find targets
            MessageSubscription[] targets;
            lock (subscriptions)
                targets = subscriptions.Where(x => x.Name == message.Name.ToUpper()).ToArray();

            //Dispatch to subscriptions
            foreach (var t in targets)
                t.Event.Invoke(message, isProcessingGroup);
        }

        private void ProcessBeginGroup()
        {
            //Set flag
            isProcessingGroup = true;

            //Fire event
            OnGroupProcessingBegin?.Invoke(this);
        }

        private void ProcessEndGroup()
        {
            //Clear flag
            isProcessingGroup = false;

            //Fire event
            OnGroupProcessingEnd?.Invoke(this);
        }

        private void ProcessVer(LWRPMessage message)
        {
            lock (mutex)
            {
                //Clear out items
                infoLwrpVer = null;
                infoDeviceName = null;
                infoDeviceVer = null;
                infoDeviceModel = null;
                infoSrcNum = 0;
                infoDstNum = 0;
                infoGpiNum = 0;
                infoGpoNum = 0;

                //Decode arguments
                LWRPToken token;
                if (message.TryGetNamedArgument("LWRP", out token))
                    infoLwrpVer = token.Content;
                if (message.TryGetNamedArgument("DEVN", out token))
                    infoDeviceName = token.Content;
                if (message.TryGetNamedArgument("SYSV", out token))
                    infoDeviceVer = token.Content;
                if (message.TryGetNamedArgument("SVER", out token)) // not documented, but appears on xNodes rather then SYSV
                    infoDeviceVer = token.Content;
                if (message.TryGetNamedArgument("MODEL", out token)) // not documented, but appears on xNodes rather then SYSV
                    infoDeviceModel = token.Content;
                if (message.TryGetNamedArgument("NSRC", out token))
                    infoSrcNum = int.Parse(token.Content);
                if (message.TryGetNamedArgument("NDST", out token))
                    infoDstNum = int.Parse(token.Content);
                if (message.TryGetNamedArgument("NGPI", out token))
                    infoGpiNum = int.Parse(token.Content);
                if (message.TryGetNamedArgument("NGPO", out token))
                    infoGpoNum = int.Parse(token.Content);

                //Set flag
                hasInfoData = true;

                //Send event
                OnInfoDataReceived?.Invoke(this);

                //Update state
                State = LWRPState.READY;

                //Fire ready task completion, if we haven't yet
                if (!readyTask.Task.IsCompleted)
                    readyTask.SetResult(this);
            }
        }

        /// <summary>
        /// Holds a subscription to get events when reciving certain messages.
        /// </summary>
        class MessageSubscription
        {
            public MessageSubscription(string name, MessageSubscriptionEvent evt)
            {
                this.name = name.ToUpper();
                this.evt = evt;
            }

            private readonly string name;
            private readonly MessageSubscriptionEvent evt;

            /// <summary>
            /// The name of the message this is subscribed to
            /// </summary>
            public string Name => name;

            /// <summary>
            /// The event to raise.
            /// </summary>
            public MessageSubscriptionEvent Event => evt;
        }
    }
}
