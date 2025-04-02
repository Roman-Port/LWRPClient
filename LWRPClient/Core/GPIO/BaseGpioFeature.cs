using LWRPClient.Exceptions;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.GPIO
{
    abstract class BaseGpioFeature<T> : BatchUpdateFeature<T>
    {
        protected BaseGpioFeature(LWRPConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Gets the count from the connection without first seeing if it is available.
        /// </summary>
        protected abstract int CountUnchecked { get; }

        /// <summary>
        /// Gets the array of ports from the user.
        /// </summary>
        protected abstract BaseGpioPort[] PortsImpl { get; }

        /// <summary>
        /// The number of GPIOs available on the device, checking if this is valid yet.
        /// </summary>
        public int Count
        {
            get
            {
                //Make sure data is available
                if (!connection.HasInfoData)
                    throw new InfoDataNotReadyException();
                return CountUnchecked;
            }
        }

        /// <summary>
        /// Gets a GPO port from an index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                //Make sure the index is valid. This also ensures info data is ready
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (T)(object)PortsImpl[index];
            }
        }

        /// <summary>
        /// Check that all pins have their states before marking as ready
        /// </summary>
        /// <returns></returns>
        protected override bool IsReady()
        {
            //Check that we know how many pins there are...
            if (!connection.HasInfoData)
                return false;

            //Check that all pins are ready
            for (int i = 0; i < Count; i++)
            {
                if (!PortsImpl[i].IsReady())
                    return false;
            }

            return base.IsReady();
        }

        /// <summary>
        /// Handles state changes. Bound by the client.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inGroup"></param>
        protected void OnStateUpdateMessageReceived(LWRPMessage message, bool inGroup)
        {
            //Get the GPO index (starting at 1)
            int index = int.Parse(message.Arguments[0][0].Content);
            BaseGpioPort port = PortsImpl[index - 1];

            //Get the pin states
            char[] pinsRaw = message.Arguments[1][0].Content.ToCharArray();

            //Apply to port
            port.StateUpdateRecieved(pinsRaw);

            //Enqueue for batch update
            EnqueueReceivedUpdate((T)(object)port, inGroup);
        }

        /// <summary>
        /// A GPIO port
        /// </summary>
        protected abstract class BaseGpioPort : LwPropertyItem
        {
            public BaseGpioPort(LWRPConnection connection, int index)
            {
                this.connection = connection;
                this.index = index;
            }

            protected readonly LWRPConnection connection;
            protected readonly int index; // starting from 1
            protected readonly LWRPPinState[] pins = new LWRPPinState[5];
            private bool hasStateData = false; // True when state data has been recieved from the server.

            /// <summary>
            /// Index of the port, starting at 1.
            /// </summary>
            public int Index => index;

            /// <summary>
            /// Access to the pins.
            /// </summary>
            public abstract ILWRPPins Pins { get; }

            /// <summary>
            /// True when all data has been recieved from the server.
            /// </summary>
            /// <returns></returns>
            public virtual bool IsReady()
            {
                return hasStateData;
            }

            /// <summary>
            /// Fired when the state of the pins is updated from the server.
            /// rawStates is an array of 5 characters for each pin representing the state, sent by the server.
            /// </summary>
            public void StateUpdateRecieved(char[] pinsRaw)
            {
                lock (pins)
                {
                    //Validate
                    if (pinsRaw.Length != 5)
                        throw new Exception($"Server sent invalid data: Expected 5 GPI pins, got {pinsRaw.Length} instead!");

                    //Decode pin states
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

                    //Set flag
                    hasStateData = true;
                }
            }
        }
    }
}
