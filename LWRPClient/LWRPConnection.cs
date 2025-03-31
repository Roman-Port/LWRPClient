using LWRPClient.Entities;
using LWRPClient.Exceptions;
using LWRPClient.Layer1.Transports;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LWRPClient
{
    public class LWRPConnection : IDisposable
    {
        public delegate void InfoDataReceivedEventArgs(LWRPConnection conn);
        public delegate void SrcBatchUpdateEventArgs(LWRPConnection conn, ILWRPSource[] sources);
        public delegate void DstBatchUpdateEventArgs(LWRPConnection conn, ILWRPDestination[] destinations);
        public delegate void ConnectionStateUpdateEventArgs(LWRPConnection conn, LWRPState state);

        public LWRPConnection(ILWRPTransport transport)
        {
            //Set transport and map events
            this.transport = transport;
            transport.OnConnected += Transport_OnConnected;
            transport.OnDisconnected += Transport_OnDisconnected;
            transport.OnMessageReceived += Transport_OnMessageReceived;

            //Create sources
            for (int i = 0; i < sources.Length; i++)
                sources[i] = new LwSrc(this, i + 1);

            //Create destinations
            for (int i = 0; i < destinations.Length; i++)
                destinations[i] = new LwDst(this, i + 1);

            //Misc
            readyTask = new TaskCompletionSource<LWRPConnection>();
        }

        public LWRPConnection(IPAddress address, int port = 93) : this(new TcpLWRPTransport(new IPEndPoint(address, port)))
        {

        }

        private readonly ILWRPTransport transport;
        private readonly object mutex = new object();
        private readonly LwSrc[] sources = new LwSrc[256];
        private readonly LwDst[] destinations = new LwDst[256];

        /// <summary>
        /// Sources that have been updated in the current event group being processed
        /// </summary>
        private readonly List<LwSrc> pendingUpdateSources = new List<LwSrc>();

        /// <summary>
        /// Destinations that have been updated in the current event group being processed
        /// </summary>
        private readonly List<LwDst> pendingUpdateDestinations = new List<LwDst>();

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
        /// Bit flag of all first-time connect parts that are required to consider this connection "ready"
        /// </summary>
        private int initInfoRecieved = 0;

        /// <summary>
        /// Task that is completed when all info data is recieved for the first time.
        /// </summary>
        private TaskCompletionSource<LWRPConnection> readyTask;

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
        /// LWRP sources on this device.
        /// </summary>
        public ILWRPSource[] Sources
        {
            get
            {
                ILWRPSource[] result;
                lock (mutex)
                {
                    //Make sure we have info data
                    if (!hasInfoData)
                        throw new InfoDataNotReadyException();

                    //Create array of this size and copy into it
                    result = new ILWRPSource[SrcNum];
                    Array.Copy(sources, result, SrcNum);
                }
                return result;
            }
        }

        /// <summary>
        /// LWRP destinations on this device.
        /// </summary>
        public ILWRPDestination[] Destinations
        {
            get
            {
                ILWRPDestination[] result;
                lock (mutex)
                {
                    //Make sure we have info data
                    if (!hasInfoData)
                        throw new InfoDataNotReadyException();

                    //Create array of this size and copy into it
                    result = new ILWRPDestination[DstNum];
                    Array.Copy(destinations, result, DstNum);
                }
                return result;
            }
        }

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
        public event SrcBatchUpdateEventArgs OnSrcBatchUpdate;
        public event DstBatchUpdateEventArgs OnDstBatchUpdate;
        public event ConnectionStateUpdateEventArgs OnConnectionStateUpdate;

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
        /// Resets state before connecting
        /// </summary>
        private void ResetState()
        {
            //Reset state to initializing
            State = LWRPState.CONNECTING;

            //Clear gathered data because we will be fetching it again
            initInfoRecieved = 0;
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
                //Loop through all and add any 
                for (int i = 0; i < infoSrcNum; i++)
                {
                    if (sources[i].CreateUpdateMessage(out LWRPMessage msg))
                        updates.Add(msg);
                }
                for (int i = 0; i < infoDstNum; i++)
                {
                    if (destinations[i].CreateUpdateMessage(out LWRPMessage msg))
                        updates.Add(msg);
                }

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

        /// <summary>
        /// Sets a bit in the init bitfield as we gather initialization data. When we have all parts, this automatically updates state to "ready".
        /// </summary>
        private void SetInitDataFlag(InitDataBit bit)
        {
            lock (mutex)
            {
                //Set bit and see if it changes anything
                int updated = initInfoRecieved | (1 << (int)bit);
                if (updated == initInfoRecieved)
                    return;

                //Update
                initInfoRecieved = updated;

                //If all are ready, set to ready
                if (updated == 0b111)
                {
                    //Update state
                    State = LWRPState.READY;

                    //Fire ready task completion, if we haven't yet
                    if (!readyTask.Task.IsCompleted)
                        readyTask.SetResult(this);
                }
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
                case "SRC": ProcessSrc(message); break;
                case "DST": ProcessDst(message); break;
            }
        }

        private void ProcessBeginGroup()
        {
            //Set flag
            isProcessingGroup = true;
        }

        private void ProcessEndGroup()
        {
            //Clear flag
            isProcessingGroup = false;

            //Check if there are results in sources or destinations for setting initialization flags. We want to set these after we send events though
            bool hasPendingSrcs = pendingUpdateSources.Count > 0;
            bool hasPendingDsts = pendingUpdateDestinations.Count > 0;

            //Send batch updates for sources
            if (pendingUpdateSources.Count > 0)
            {
                OnSrcBatchUpdate?.Invoke(this, pendingUpdateSources.ToArray());
                pendingUpdateSources.Clear();
            }

            //Send batch updates for destinations
            if (pendingUpdateDestinations.Count > 0)
            {
                OnDstBatchUpdate?.Invoke(this, pendingUpdateDestinations.ToArray());
                pendingUpdateDestinations.Clear();
            }

            //Update init flags
            if (hasPendingDsts)
                SetInitDataFlag(InitDataBit.DST);
            if (hasPendingSrcs)
                SetInitDataFlag(InitDataBit.SRC);
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

                //Set init flag
                SetInitDataFlag(InitDataBit.VER);

                //Send SRC and DST to get info about the sources and destinations
                transport.SendMessage(new LWRPMessage("SRC", new LWRPToken[0][]));
                transport.SendMessage(new LWRPMessage("DST", new LWRPToken[0][]));
            }
        }

        private void ProcessSrc(LWRPMessage message)
        {
            //Get the source index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            LwSrc source = sources[index - 1];

            //Dispatch
            lock (mutex)
            {
                //Process update
                source.ProcessUpdate(message);

                //Add to event list
                if (!pendingUpdateSources.Contains(source))
                    pendingUpdateSources.Add(source);

                //If NOT in a group, send a batch update now. Otherwise it'll be sent when we end the group
                if (!isProcessingGroup)
                {
                    OnSrcBatchUpdate?.Invoke(this, pendingUpdateSources.ToArray());
                    pendingUpdateSources.Clear();
                    SetInitDataFlag(InitDataBit.SRC);
                }
            }
        }

        private void ProcessDst(LWRPMessage message)
        {
            //Get the source index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            LwDst dst = destinations[index - 1];

            //Dispatch
            lock (mutex)
            {
                //Process update
                dst.ProcessUpdate(message);

                //Add to event list
                if (!pendingUpdateDestinations.Contains(dst))
                    pendingUpdateDestinations.Add(dst);

                //If NOT in a group, send a batch update now. Otherwise it'll be sent when we end the group
                if (!isProcessingGroup)
                {
                    OnDstBatchUpdate?.Invoke(this, pendingUpdateDestinations.ToArray());
                    pendingUpdateDestinations.Clear();
                    SetInitDataFlag(InitDataBit.DST);
                }
            }
        }

        /* DATA TYPES */

        private enum InitDataBit
        {
            VER = 0,
            SRC = 1,
            DST = 2
        }

        abstract class LwIoItem
        {
            public LwIoItem(LWRPConnection conn, int index)
            {
                this.conn = conn;
                this.index = index;
            }

            protected readonly LWRPConnection conn;
            protected readonly int index; //starting at 1
            protected bool hasInfoData;

            /// <summary>
            /// Latest info message describing this item
            /// </summary>
            private LWRPMessage info;

            /// <summary>
            /// Changes pending to be applied
            /// </summary>
            private Dictionary<string, LWRPToken> pendingChanges = new Dictionary<string, LWRPToken>();

            /// <summary>
            /// Index, starting at 1.
            /// </summary>
            public int Index => index;

            /// <summary>
            /// The name of the command used to apply changes
            /// </summary>
            protected abstract string MessageName { get; }

            public void ProcessUpdate(LWRPMessage message)
            {
                info = message;
                hasInfoData = true;
            }

            protected bool TryReadProperty(string key, out LWRPToken token)
            {
                //If no data yet, reject
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();

                //Search for it in pending changes first
                /*if (pendingChanges.TryGetValue(key, out token))
                    return true;*/

                //Next find it in the info
                if (info.TryGetNamedArgument(key, out token))
                    return true;

                return false;
            }

            protected bool TryReadProperty(string key, out string token)
            {
                if (TryReadProperty(key, out LWRPToken raw))
                {
                    token = raw.Content;
                    return true;
                }
                token = null;
                return false;
            }

            protected string ReadPropertyString(string key, string defaultValue = null)
            {
                if (TryReadProperty(key, out string token))
                    return token;
                return defaultValue;
            }

            protected bool TryReadProperty(string key, out int token)
            {
                if (TryReadProperty(key, out LWRPToken raw))
                {
                    token = int.Parse(raw.Content);
                    return true;
                }
                token = 0;
                return false;
            }

            protected int ReadPropertyInt(string key, int defaultValue = 0)
            {
                if (TryReadProperty(key, out int token))
                    return token;
                return defaultValue;
            }

            protected bool TryReadProperty(string key, out bool token)
            {
                if (TryReadProperty(key, out int raw))
                {
                    if (raw == 0)
                        token = false;
                    else if (raw == 1)
                        token = true;
                    else
                        throw new Exception("Invalid boolean value.");
                    return true;
                }
                token = false;
                return false;
            }

            protected bool ReadPropertyBool(string key, bool defaultValue = false)
            {
                if (TryReadProperty(key, out bool token))
                    return token;
                return defaultValue;
            }

            protected void SetProperty(string key, LWRPToken token)
            {
                lock (pendingChanges)
                {
                    if (pendingChanges.ContainsKey(key))
                        pendingChanges[key] = token;
                    else
                        pendingChanges.Add(key, token);
                }
            }

            /// <summary>
            /// Encapsulates updates pending into a message and clears them pending. Returns if it was successful.
            /// </summary>
            /// <returns></returns>
            public bool CreateUpdateMessage(out LWRPMessage msg)
            {
                lock (pendingChanges)
                {
                    //If no updates, return null
                    if (pendingChanges.Count == 0)
                    {
                        msg = null;
                        return false;
                    }

                    //Create the arguments
                    LWRPToken[][] tokens = new LWRPToken[pendingChanges.Count + 1][];

                    //The first token is the index
                    tokens[0] = new LWRPToken[] { new LWRPToken(false, index.ToString()) };

                    //Add each of the arguments
                    int i = 1;
                    foreach (var c in pendingChanges)
                        tokens[i++] = new LWRPToken[] { new LWRPToken(false, c.Key), c.Value };

                    //Clear pending changes
                    pendingChanges.Clear();

                    msg = new LWRPMessage(MessageName, tokens);
                    return true;
                }
            }
        }
        
        class LwSrc : LwIoItem, ILWRPSource
        {
            public LwSrc(LWRPConnection conn, int index) : base(conn, index)
            {
                
            }

            protected override string MessageName => "SRC";

            public string PrimarySourceName
            {
                get => ReadPropertyString("PSNM");
                set => SetProperty("PSNM", new LWRPToken(true, value));
            }

            public bool RtpStreamEnabled
            {
                get => ReadPropertyBool("RTPE");
                set => SetProperty("RTPE", new LWRPToken(false, value ? "1" : "0"));
            }

            public string RtpStreamAddress
            {
                get => ReadPropertyString("RTPA");
                set => SetProperty("RTPA", new LWRPToken(true, value));
            }

            public int ChannelCount
            {
                get => ReadPropertyInt("NCHN", 2);
                set => SetProperty("NCHN", new LWRPToken(false, value.ToString()));
            }

            public int Gain
            {
                get => ReadPropertyInt("INGN");
                set => SetProperty("INGN", new LWRPToken(false, value.ToString()));
            }

            public string LcdLabel
            {
                get => ReadPropertyString("LABL");
                set => SetProperty("LABL", new LWRPToken(true, value));
            }
        }

        class LwDst : LwIoItem, ILWRPDestination
        {
            public LwDst(LWRPConnection conn, int index) : base(conn, index)
            {
            }

            protected override string MessageName => "DST";

            public string Name
            {
                get => ReadPropertyString("NAME");
                set => SetProperty("NAME", new LWRPToken(true, value));
            }

            public string Address
            {
                get => ReadPropertyString("ADDR");
                set => SetProperty("ADDR", new LWRPToken(true, value));
            }

            public int ChannelCount
            {
                get => ReadPropertyInt("NCHN", 2);
                set => SetProperty("NCHN", new LWRPToken(false, value.ToString()));
            }

            public LwChannel Channel
            {
                get => LwChannel.FromIpAddress(Address);
                set
                {
                    if (value == null || value.Type == LwChannelType.INVALID)
                        Address = "";
                    else
                        Address = value.ToIpAddress().ToString();
                }
            }
        }
    }
}
