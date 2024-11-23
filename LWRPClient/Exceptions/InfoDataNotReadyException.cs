using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Exceptions
{
    public class InfoDataNotReadyException : Exception
    {
        public InfoDataNotReadyException() : base("Info data has not yet been received.")
        {

        }
    }
}
