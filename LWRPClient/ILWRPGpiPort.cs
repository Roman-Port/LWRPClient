﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    /// <summary>
    /// A GPI (to network) port containing 5 pins. 
    /// </summary>
    public interface ILWRPGpiPort
    {
        /// <summary>
        /// The index of this port, starting at 1.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The pins on this port.
        /// </summary>
        ILWRPPins Pins { get; }
    }
}
