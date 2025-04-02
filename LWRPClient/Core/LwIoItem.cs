using LWRPClient.Exceptions;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core
{
    /// <summary>
    /// A LwPropertyItem abstracted away to specific messages.
    /// </summary>
    abstract class LwIoItem : LwPropertyItem
    {
        public LwIoItem(LWRPConnection conn, int index)
        {
            this.conn = conn;
            this.index = index;
        }

        protected readonly LWRPConnection conn;
        protected readonly int index; //starting at 1

        /// <summary>
        /// Index, starting at 1.
        /// </summary>
        public int Index => index;

        /// <summary>
        /// The name of the command used to apply changes
        /// </summary>
        protected abstract string MessageName { get; }

        /// <summary>
        /// Processes an update from the server.
        /// </summary>
        /// <param name="message"></param>
        public void ProcessUpdate(LWRPMessage message)
        {
            //Ingest messages, skipping the index.
            ProcessServerPropertyUpdate(message.Arguments, 1, message.Arguments.Length - 1);
        }

        /// <summary>
        /// Encapsulates updates pending into a message and clears them pending. Returns if it was successful.
        /// </summary>
        /// <returns></returns>
        public bool CreateUpdateMessage(out LWRPMessage msg)
        {
            //Start building...the first token is the index
            List<LWRPToken[]> tokens = new List<LWRPToken[]>();
            tokens.Add(new LWRPToken[] { new LWRPToken(false, index.ToString()) });

            //Add arguments
            if (!CreateUpdateMessageProperties(tokens))
            {
                //No changes
                msg = null;
                return false;
            }

            //Wrap
            msg = new LWRPMessage(MessageName, tokens.ToArray());
            return true;
        }
    }
}
