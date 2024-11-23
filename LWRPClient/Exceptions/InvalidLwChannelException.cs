using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Exceptions
{
    public class InvalidLwChannelException : Exception
    {
        public InvalidLwChannelException() : base("Unable to perform operations on invalid Livewire channel.")
        {

        }
    }
}
