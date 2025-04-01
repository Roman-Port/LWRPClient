using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    public interface ILWRPGpiPort
    {
        /// <summary>
        /// The index of this port, starting at 1.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The pins on this port.
        /// </summary>
        LWRPPinState[] Pins { get; }
    }
}
