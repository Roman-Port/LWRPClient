using LWRPClient.Exceptions;
using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.GPIO
{
    /// <summary>
    /// Feature for GPO (from network).
    /// </summary>
    class GpoFeature : BaseGpioFeature<ILWRPGpoPort>, ILWRPGpoFeature
    {
        public GpoFeature(LWRPConnection connection) : base(connection)
        {
            //Create ports
            for (int i = 0; i < ports.Length; i++)
                ports[i] = new GpoPort(connection, i + 1);

            //Bind to events
            connection.OnInfoDataReceived += Connection_OnInfoDataReceived;
            connection.SubscribeToMessage("GPO", OnStateUpdateMessageReceived);
            connection.SubscribeToMessage("CFG", OnConfigUpdateMessageReceived);
        }

        private readonly GpoPort[] ports = new GpoPort[256];

        /// <summary>
        /// Gets the count from the connection without first seeing if it is available.
        /// </summary>
        protected override int CountUnchecked => connection.GpoNum;

        /// <summary>
        /// Gets the array of ports from the user.
        /// </summary>
        protected override BaseGpioPort[] PortsImpl => ports;

        private void Connection_OnInfoDataReceived(LWRPConnection conn)
        {
            //Check if the device supports GPO. If it doesn't this will return a bad command error
            if (Count > 0)
            {
                //Request state changes; This also subscribes us to updates
                conn.transport.SendMessage(new LWRPMessage("ADD", new LWRPToken[][]
                {
                    new LWRPToken[] { new LWRPToken(false, "GPO") }
                }));

                //Then, request config
                conn.transport.SendMessage(new LWRPMessage("CFG", new LWRPToken[][]
                {
                    new LWRPToken[] { new LWRPToken(false, "GPO") }
                }));
            }
            else
            {
                //Mark as ready immediately
                MarkAsReady();
            }
        }

        /// <summary>
        /// Handles CFG commands for getting config of a GPO.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inGroup"></param>
        protected void OnConfigUpdateMessageReceived(LWRPMessage message, bool inGroup)
        {
            //Check that the config type verb is "GPO"
            if (message.Arguments[0][0].Content.ToUpper() != "GPO")
                return;

            //Get the GPO index (starting at 1)
            int index = int.Parse(message.Arguments[1][0].Content);
            GpoPort port = ports[index - 1];

            //Apply changes to property to port, omitting header data
            port.ProcessServerPropertyUpdate(message.Arguments, 2, message.Arguments.Length - 2);

            //Enqueue for batch update
            EnqueueReceivedUpdate(port, inGroup);
        }

        internal override void Apply(IList<LWRPMessage> updates)
        {
            //Apply all state changes
            for (int i = 0; i < connection.GpoNum; i++)
                ports[i].ApplyStateChanges(updates);

            //Apply all property changes
            for (int i = 0; i < connection.GpoNum; i++)
                ports[i].ApplyPropertyChanges(updates);
        }

        class GpoPort : BaseGpioPort, ILWRPGpoPort
        {
            public GpoPort(LWRPConnection connection, int index) : base(connection, index)
            {
                //Create adapter
                adapter = new PinAdapter(this);
            }

            /// <summary>
            /// Adapter for accessing the port.
            /// </summary>
            private readonly PinAdapter adapter;

            /// <summary>
            /// The pins that have been updated from the client side.
            /// </summary>
            private readonly LWRPPinState[] clientPinUpdates = new LWRPPinState[5];

            /// <summary>
            /// A bitmask of pins that have been changed from the client side.
            /// A set bit indicates that clientPinUpdates should be read, otherwise read from the server-side pins.
            /// </summary>
            private int clientPinUpdatesMask;

            /// <summary>
            /// Accesses the adapter for reading/writing pins.
            /// </summary>
            public override ILWRPPins Pins => adapter;

            /// <summary>
            /// Returns true when there are client-side changes pending.
            /// </summary>
            public bool ChangesPending
            {
                get
                {
                    bool result;
                    lock (pins)
                        result = clientPinUpdatesMask != 0;
                    return result;
                }
            }

            /// <summary>
            /// The user-defined name of this port.
            /// </summary>
            public string Name
            {
                get => ReadPropertyString("NAME");
                set => SetProperty("NAME", new LWRPToken(true, value));
            }

            /// <summary>
            /// The source address.
            /// </summary>
            public string SourceAddress
            {
                get => ReadPropertyString("SRCA");
                set => SetProperty("SRCA", new LWRPToken(true, value));
            }

            /// <summary>
            /// If there are any state changes, creates a message to apply them to the server and adds it to the list.
            /// Additionally, resets the client side state.
            /// </summary>
            /// <param name="updates"></param>
            public void ApplyStateChanges(IList<LWRPMessage> updates)
            {
                lock (pins)
                {
                    //Only do anything if any pins are set
                    if (clientPinUpdatesMask != 0)
                    {
                        //Create the state changes array. This is like the incoming one, except 'x' indicates no change.
                        //'x' is used for all pins that were not set by the client
                        char[] stateChanges = new char[5];
                        for (int i = 0; i < 5; i++)
                        {
                            //Check if this is a user-modified value...
                            char value = 'x';
                            if ((clientPinUpdatesMask & (1 << i)) != 0)
                            {
                                //Client side update
                                switch (clientPinUpdates[i])
                                {
                                    case LWRPPinState.FALLING: value = 'L'; break;
                                    case LWRPPinState.LOW: value = 'l'; break;
                                    case LWRPPinState.HIGH: value = 'h'; break;
                                    case LWRPPinState.RISING: value = 'H'; break;
                                    default: throw new Exception($"Invalid pin: '{clientPinUpdates[i]}' is not a valid pin state for pin #{i + 1}.");
                                }
                            }

                            //Set
                            stateChanges[i] = value;
                        }

                        //Create the command. It IS possible to specify a delay but that applies to all pins and only when applied. This would be difficult to make an API for?
                        updates.Add(new LWRPMessage("GPO", new LWRPToken[][]
                        {
                            new LWRPToken[] { new LWRPToken(false, index.ToString()) },
                            new LWRPToken[] { new LWRPToken(false, new string(stateChanges)) }
                        }));

                        //Reset mask to clear client-side changes
                        clientPinUpdatesMask = 0;
                    }
                }
            }

            /// <summary>
            /// Since this also processes config data, we need to make sure that's recieved to mark this as ready.
            /// </summary>
            /// <returns></returns>
            public override bool IsReady()
            {
                return base.IsReady() && PropertyDataReceived;
            }

            /// <summary>
            /// If any property changes are needed, pushes the message to the messages list.
            /// </summary>
            /// <param name="messages"></param>
            public void ApplyPropertyChanges(IList<LWRPMessage> messages)
            {
                //Start building
                List<LWRPToken[]> tokens = new List<LWRPToken[]>();
                tokens.Add(new LWRPToken[] { new LWRPToken(false, "GPO") }); // First token is "GPO"
                tokens.Add(new LWRPToken[] { new LWRPToken(false, index.ToString()) }); // Second token is index

                //Add arguments
                if (!CreateUpdateMessageProperties(tokens))
                    return; // No changes

                //Wrap into message and add to list
                messages.Add(new LWRPMessage("CFG", tokens.ToArray()));
            }

            /// <summary>
            /// Adapter that provides access to the port.
            /// </summary>
            class PinAdapter : ILWRPPins
            {
                public PinAdapter(GpoPort port)
                {
                    this.port = port;
                }

                private readonly GpoPort port;

                public LWRPPinState this[int index]
                {
                    get
                    {
                        LWRPPinState state;
                        lock (port.pins)
                        {
                            //If this pin is within the modified mask, read the client-side value. Otherwise, read the server-side one.
                            //This lets the client see pending changes
                            if ((port.clientPinUpdatesMask & (1 << index)) != 0)
                                state = port.clientPinUpdates[index];
                            else
                                state = port.pins[index];
                        }
                        return state;
                    }
                    set
                    {
                        //Validate
                        if (value != LWRPPinState.RISING && value != LWRPPinState.HIGH && value != LWRPPinState.LOW && value != LWRPPinState.FALLING)
                            throw new ArgumentException("Invalid pin state.");

                        //Enter mutex
                        lock (port.pins)
                        {
                            //Set the value in the clientPinUpdates, then apply it to the mask as well
                            port.clientPinUpdates[index] = value;
                            port.clientPinUpdatesMask |= (1 << index);
                        }
                    }
                }

                public bool ReadOnly => false;

                public LWRPPinState[] ToArray()
                {
                    LWRPPinState[] result = new LWRPPinState[5];
                    lock (port.pins)
                    {
                        //Perform a manual copy while within the mutex
                        for (int i = 0; i < 5; i++)
                            result[i] = this[i];
                    }
                    return result;
                }
            }
        }
    }
}
