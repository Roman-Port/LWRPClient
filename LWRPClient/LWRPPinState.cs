using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient
{
    /// <summary>
    /// Pin state for GPIO. Remember that high is the "default" value.
    /// </summary>
    public enum LWRPPinState : int
    {
        FALLING = -2,
        LOW = -1,

        HIGH = 1,
        RISING = 2
    }
}
