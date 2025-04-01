using LWRPClient.Exceptions;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core
{
    abstract class LwIoItem
    {
        public LwIoItem(LWRPConnection conn, int index)
        {
            this.conn = conn;
            this.index = index;
        }

        protected readonly LWRPConnection conn;
        protected readonly int index; //starting at 1
        protected bool hasInfoData;

        /// <summary>
        /// Latest info message describing this item
        /// </summary>
        private LWRPMessage info;

        /// <summary>
        /// Changes pending to be applied
        /// </summary>
        private Dictionary<string, LWRPToken> pendingChanges = new Dictionary<string, LWRPToken>();

        /// <summary>
        /// Index, starting at 1.
        /// </summary>
        public int Index => index;

        /// <summary>
        /// The name of the command used to apply changes
        /// </summary>
        protected abstract string MessageName { get; }

        public void ProcessUpdate(LWRPMessage message)
        {
            info = message;
            hasInfoData = true;
        }

        protected bool TryReadProperty(string key, out LWRPToken token)
        {
            //If no data yet, reject
            if (!hasInfoData)
                throw new InfoDataNotReadyException();

            //Search for it in pending changes first
            /*if (pendingChanges.TryGetValue(key, out token))
                return true;*/

            //Next find it in the info
            if (info.TryGetNamedArgument(key, out token))
                return true;

            return false;
        }

        protected bool TryReadProperty(string key, out string token)
        {
            if (TryReadProperty(key, out LWRPToken raw))
            {
                token = raw.Content;
                return true;
            }
            token = null;
            return false;
        }

        protected string ReadPropertyString(string key, string defaultValue = null)
        {
            if (TryReadProperty(key, out string token))
                return token;
            return defaultValue;
        }

        protected bool TryReadProperty(string key, out int token)
        {
            if (TryReadProperty(key, out LWRPToken raw))
            {
                token = int.Parse(raw.Content);
                return true;
            }
            token = 0;
            return false;
        }

        protected int ReadPropertyInt(string key, int defaultValue = 0)
        {
            if (TryReadProperty(key, out int token))
                return token;
            return defaultValue;
        }

        protected bool TryReadProperty(string key, out bool token)
        {
            if (TryReadProperty(key, out int raw))
            {
                if (raw == 0)
                    token = false;
                else if (raw == 1)
                    token = true;
                else
                    throw new Exception("Invalid boolean value.");
                return true;
            }
            token = false;
            return false;
        }

        protected bool ReadPropertyBool(string key, bool defaultValue = false)
        {
            if (TryReadProperty(key, out bool token))
                return token;
            return defaultValue;
        }

        protected void SetProperty(string key, LWRPToken token)
        {
            lock (pendingChanges)
            {
                if (pendingChanges.ContainsKey(key))
                    pendingChanges[key] = token;
                else
                    pendingChanges.Add(key, token);
            }
        }

        /// <summary>
        /// Encapsulates updates pending into a message and clears them pending. Returns if it was successful.
        /// </summary>
        /// <returns></returns>
        public bool CreateUpdateMessage(out LWRPMessage msg)
        {
            lock (pendingChanges)
            {
                //If no updates, return null
                if (pendingChanges.Count == 0)
                {
                    msg = null;
                    return false;
                }

                //Create the arguments
                LWRPToken[][] tokens = new LWRPToken[pendingChanges.Count + 1][];

                //The first token is the index
                tokens[0] = new LWRPToken[] { new LWRPToken(false, index.ToString()) };

                //Add each of the arguments
                int i = 1;
                foreach (var c in pendingChanges)
                    tokens[i++] = new LWRPToken[] { new LWRPToken(false, c.Key), c.Value };

                //Clear pending changes
                pendingChanges.Clear();

                msg = new LWRPMessage(MessageName, tokens);
                return true;
            }
        }
    }
}
