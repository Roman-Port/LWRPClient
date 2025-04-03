using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Exceptions
{
    /// <summary>
    /// Exception thrown when processing data from the server that is invalid.
    /// </summary>
    public class InvalidResponseException : Exception
    {
        public InvalidResponseException(string message, LWRPMessage command) : base($"Server sent invalid data: {message}")
        {
            this.command = command;
        }

        private readonly LWRPMessage command;

        /// <summary>
        /// The offending command recieved from the server.
        /// </summary>
        public LWRPMessage Command => command;
    }
}
