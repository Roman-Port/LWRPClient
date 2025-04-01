using LWRPClient.Features;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.GPIO
{
    class GpiFeature : BatchUpdateFeature<ILWRPGpiPort>, ILWRPGpiFeature
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

            //Validate
            if (pinsRaw.Length != 5)
                throw new Exception($"Server sent invalid data: Expected 5 GPI pins, got {pinsRaw.Length} instead!");

            //Decode pin states
            LWRPPinState[] pins = new LWRPPinState[5];
            for (int i = 0; i < 5; i++)
            {
                LWRPPinState state;
                switch (pinsRaw[i])
                {
                    case 'L': state = LWRPPinState.FALLING; break;
                    case 'l': state = LWRPPinState.LOW; break;
                    case 'h': state = LWRPPinState.HIGH; break;
                    case 'H': state = LWRPPinState.RISING; break;
                    default: throw new Exception($"Server sent invalid data: '{pinsRaw[i]}' is not a valid pin state for pin #{i + 1}.");
                }
                pins[i] = state;
            }

            //Apply to port
            port.StateUpdateRecieved(pins);

            //Enqueue for batch update
            EnqueueReceivedUpdate(port, inGroup);
        }

        internal override void Apply(IList<LWRPMessage> updates)
        {
            throw new NotImplementedException();
        }

        class GpiPort : ILWRPGpiPort
        {
            public GpiPort(LWRPConnection connection, int index)
            {
                this.connection = connection;
                this.index = index;
            }

            private readonly LWRPConnection connection;
            private readonly int index; // starting from 1
            private readonly LWRPPinState[] pins = new LWRPPinState[5];

            public int Index => index;

            public LWRPPinState[] Pins => pins;

            /// <summary>
            /// Fired when the state of the pins is updated from the server.
            /// </summary>
            public void StateUpdateRecieved(LWRPPinState[] newState)
            {
                //Copy
                Array.Copy(newState, pins, 5);
            }
        }
    }
}
