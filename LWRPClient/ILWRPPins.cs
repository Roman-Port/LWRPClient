using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    /// <summary>
    /// Represents a collection of 5 pins.
    /// </summary>
    public interface ILWRPPins
    {
        /// <summary>
        /// The value of these pins.
        /// </summary>
        /// <returns></returns>
        LWRPPinState this[int index]
        {
            get;
            set;
        }

        /// <summary>
        /// True if pins cannot be modified.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Returns an array of the 5 pins.
        /// </summary>
        /// <returns></returns>
        LWRPPinState[] ToArray();
    }
}
