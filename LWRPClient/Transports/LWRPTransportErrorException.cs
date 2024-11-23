using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Transports
{
    public class LWRPTransportErrorException : Exception
    {
        public LWRPTransportErrorException(string message) : base(message)
        {

        }
    }
}
