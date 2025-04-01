using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
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
    }
}
