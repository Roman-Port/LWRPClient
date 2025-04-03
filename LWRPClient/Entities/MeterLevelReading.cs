using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Entities
{
    /// <summary>
    /// A specific reading for an audio level
    /// </summary>
    public class MeterLevelReading
    {
        public MeterLevelReading(float l, float r)
        {
            this.l = l;
            this.r = r;
        }

        private readonly float l;
        private readonly float r;

        /// <summary>
        /// Reading in dbFS for the left channel.
        /// </summary>
        public float L => l;

        /// <summary>
        /// Reading in dbFS for the right channel.
        /// </summary>
        public float R => r;

        /// <summary>
        /// Average reading between the L and R channels in dbFS.
        /// </summary>
        public float Mix => (L + R) / 2;
    }
}
