using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.GPIO
{
    class GpiFeature : BaseGpioFeature<ILWRPGpiPort>, ILWRPGpiFeature
    {
        public GpiFeature(LWRPConnection connection) : base(connection)
        {
            //Create ports
            for (int i = 0; i < ports.Length; i++)
                ports[i] = new GpiPort(connection, i + 1);

            //Bind to events
            connection.OnInfoDataReceived += Connection_OnInfoDataReceived;
            connection.SubscribeToMessage("GPI", OnUpdateMessageReceived);
        }

        private readonly GpiPort[] ports = new GpiPort[256];

        private void Connection_OnInfoDataReceived(LWRPConnection conn)
        {
            //Check if the device supports GPI. If it doesn't this will return a bad command error
            if (conn.GpiNum > 0)
            {
                //Request info; This also subscribes us to updates
                conn.transport.SendMessage(new LWRPMessage("ADD", new LWRPToken[][]
                {
                    new LWRPToken[] { new LWRPToken(false, "GPI") }
                }));
            } else
            {
                //Mark as ready immediately
                MarkAsReady();
            }
        }

        private void OnUpdateMessageReceived(LWRPMessage message, bool inGroup)
        {
            //Get the source index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            GpiPort port = ports[index - 1];

            //Get the pin states
            char[] pinsRaw = message.Arguments[1][0].Content.ToCharArray();

            //Apply to port
            port.StateUpdateRecieved(pinsRaw);

            //Enqueue for batch update
            EnqueueReceivedUpdate(port, inGroup);
        }

        internal override void Apply(IList<LWRPMessage> updates)
        {
            //Do nothing...these are read-only.
        }

        class GpiPort : BaseGpioPort, ILWRPGpiPort
        {
            public GpiPort(LWRPConnection connection, int index) : base(connection, index)
            {
                //Create adapter
                adapter = new PinAdapter(this);
            }

            private readonly PinAdapter adapter;

            public override ILWRPPins Pins => adapter;

            /// <summary>
            /// Adapter that provides access to the port.
            /// </summary>
            class PinAdapter : ILWRPPins
            {
                public PinAdapter(GpiPort port)
                {
                    this.port = port;
                }

                private readonly GpiPort port;

                public LWRPPinState this[int index]
                {
                    get
                    {
                        LWRPPinState state;
                        lock (port.pins)
                            state = port.pins[index];
                        return state;
                    }
                    set => throw new NotSupportedException();
                }

                public bool ReadOnly => true;

                public LWRPPinState[] ToArray()
                {
                    LWRPPinState[] result = new LWRPPinState[5];
                    lock (port.pins)
                        port.pins.CopyTo(result, 0);
                    return result;
                }
            }
        }
    }
}
