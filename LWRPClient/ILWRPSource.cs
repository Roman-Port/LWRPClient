using LWRPClient.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    public interface ILWRPSource
    {
        /// <summary>
        /// Index on this device starting with 1.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The main name of the source. May be null.
        /// </summary>
        string PrimarySourceName { get; set; }

        /// <summary>
        /// Enabled?
        /// </summary>
        bool RtpStreamEnabled { get; set; }

        /// <summary>
        /// The address this channel is on.
        /// </summary>
        string RtpStreamAddress { get; set; }

        /// <summary>
        /// The number of channels being sent.
        /// </summary>
        int ChannelCount { get; set; }

        /// <summary>
        /// The gain in 10 dB units.
        /// </summary>
        int Gain { get; set; }

        /// <summary>
        /// The 10-char label used on boards to show LCDs.
        /// </summary>
        string LcdLabel { get; set; }
    }
}
