using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Entities
{
    /// <summary>
    /// A reading for a meter channel.
    /// </summary>
    public class MeterChannelReading
    {
        public MeterChannelReading(MeterChannelType type, int index, MeterLevelReading peek, MeterLevelReading rms)
        {
            this.type = type;
            this.index = index;
            this.peek = peek;
            this.rms = rms;
        }

        private MeterChannelType type;
        private int index;
        private MeterLevelReading peek;
        private MeterLevelReading rms;

        /// <summary>
        /// The type of metering channel.
        /// </summary>
        public MeterChannelType Type => type;

        /// <summary>
        /// The index of the channel, starting at 1.
        /// </summary>
        public int Index => index;

        /// <summary>
        /// Peek level.
        /// </summary>
        public MeterLevelReading Peek => peek;

        /// <summary>
        /// RMS level. May be null if unavailable.
        /// </summary>
        public MeterLevelReading RMS => rms;
    }
}
