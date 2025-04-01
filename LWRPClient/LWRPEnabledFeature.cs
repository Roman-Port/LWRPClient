using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    /// <summary>
    /// Features of the LWRP client to enable.
    /// </summary>
    [Flags]
    public enum LWRPEnabledFeature
    {
        /// <summary>
        /// Sources.
        /// </summary>
        SOURCES = (1 << 0),

        /// <summary>
        /// Destinations.
        /// </summary>
        DESTINATIONS = (1 << 1),

        /// <summary>
        /// GPI (to network)
        /// </summary>
        GPI = (1 << 2),
    }
}
