using LWRPClient.Exceptions;
using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.GPIO
{
    /// <summary>
    /// Feature for GPI (to network).
    /// </summary>
    class GpiFeature : BaseGpioFeature<ILWRPGpiPort>, ILWRPGpiFeature
    {
        public GpiFeature(LWRPConnection connection) : base(connection)
        {
            //Create ports
            for (int i = 0; i < ports.Length; i++)
                ports[i] = new GpiPort(connection, i + 1);

            //Bind to events
            connection.OnInfoDataReceived += Connection_OnInfoDataReceived;
            connection.SubscribeToMessage("GPI", OnStateUpdateMessageReceived);
        }

        private readonly GpiPort[] ports = new GpiPort[256];

        /// <summary>
        /// Gets the count from the connection without first seeing if it is available.
        /// </summary>
        protected override int CountUnchecked => connection.GpiNum;

        /// <summary>
        /// Gets the array of ports from the user.
        /// </summary>
        protected override BaseGpioPort[] PortsImpl => ports;

        private void Connection_OnInfoDataReceived(LWRPConnection conn)
        {
            //Check if the device supports GPI. If it doesn't this will return a bad command error
            if (Count > 0)
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
