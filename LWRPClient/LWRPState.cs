using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    public enum LWRPState
    {
        /// <summary>
        /// User has not called Initialize function.
        /// </summary>
        PRE_INIT,

        /// <summary>
        /// Connecting or re-connecting to establish data.
        /// </summary>
        CONNECTING,

        /// <summary>
        /// Connected but still gathering data.
        /// </summary>
        INITIALIZING,

        /// <summary>
        /// Normal connection status
        /// </summary>
        READY,

        /// <summary>
        /// Connection was shut down gracefully.
        /// </summary>
        DISPOSED
    }
}
