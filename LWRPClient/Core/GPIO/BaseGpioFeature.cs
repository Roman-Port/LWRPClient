using LWRPClient.Exceptions;
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
        /// A GPIO port
        /// </summary>
        protected abstract class BaseGpioPort
        {
            public BaseGpioPort(LWRPConnection connection, int index)
            {
                this.connection = connection;
                this.index = index;
            }

            protected readonly LWRPConnection connection;
            protected readonly int index; // starting from 1
            protected readonly LWRPPinState[] pins = new LWRPPinState[5];

            public int Index => index;

            public abstract ILWRPPins Pins { get; }

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
                }
            }
        }
    }
}
