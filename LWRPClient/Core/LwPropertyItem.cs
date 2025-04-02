using LWRPClient.Exceptions;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core
{
    /// <summary>
    /// An item with properties that can be synced to the server.
    /// </summary>
    abstract class LwPropertyItem
    {
        public LwPropertyItem()
        {

        }

        /// <summary>
        /// Mutex locked while making changes.
        /// </summary>
        private readonly object mutex = new object();

        /// <summary>
        /// Server-side properties
        /// </summary>
        private readonly Dictionary<string, LWRPToken> serverProps = new Dictionary<string, LWRPToken>();

        /// <summary>
        /// Changes client-side pending to be applied
        /// </summary>
        private readonly Dictionary<string, LWRPToken> pendingChanges = new Dictionary<string, LWRPToken>();

        /// <summary>
        /// Set when ProcessServerPropertyUpdate is called to indicate data has been recieved from the server.
        /// </summary>
        private bool hasInfoData = false;

        /// <summary>
        /// Returns true if property data is available. Thread safe.
        /// </summary>
        public bool PropertyDataReceived
        {
            get
            {
                lock (mutex)
                    return hasInfoData;
            }
        }

        /// <summary>
        /// Processes incoming data from the server.
        /// Tokens is all of the settings changes (without any kind of index header data) that are interpreted as key value pairs.
        /// </summary>
        /// <param name="tokens">The tokens to read.</param>
        /// <param name="index">The number of tokens to skip in the tokens array.</param>
        /// <param name="count">The number of tokens to read.</param>
        public void ProcessServerPropertyUpdate(LWRPToken[][] tokens, int index, int count)
        {
            lock (mutex)
            {
                //Clear out server props
                serverProps.Clear();

                //Reload tokens, interpreting them as key value pairs
                LWRPToken[] t;
                for (int i = 0; i < count; i++)
                {
                    //Get token
                    t = tokens[index + i];

                    //Validate
                    if (t.Length != 2)
                        throw new Exception($"Server sent invalid data: Token array length was {t.Length} when 2 was expected to parse as a key/value pair.");

                    //Get key values
                    LWRPToken key = t[0];
                    LWRPToken value = t[1];

                    //Make sure it isn't already there
                    if (serverProps.ContainsKey(key.Content))
                        throw new Exception($"Server sent invalid data: Key \"{key.Content}\" was set more than once.");

                    //Set in property table
                    serverProps.Add(key.Content, value);
                }

                //Set flag
                hasInfoData = true;
            }
        }

        /// <summary>
        /// Creates the key values of pending server updates and adds them to the tokens out. Does not include header data.
        /// Returns true if there were any changes, otherwise false.
        /// </summary>
        /// <returns></returns>
        public bool CreateUpdateMessageProperties(IList<LWRPToken[]> tokens)
        {
            lock (mutex)
            {
                //Check if there are any changes
                if (pendingChanges.Count == 0)
                    return false;

                //Add each of the arguments
                foreach (var c in pendingChanges)
                    tokens.Add(new LWRPToken[] { new LWRPToken(false, c.Key), c.Value });

                //Clear pending changes
                pendingChanges.Clear();

                return true;
            }
        }

        protected bool TryReadProperty(string key, out LWRPToken token)
        {
            lock (mutex)
            {
                //If no data yet, reject
                if (!hasInfoData)
                    throw new InfoDataNotReadyException();

                //Search for it in pending changes first
                /*if (pendingChanges.TryGetValue(key, out token))
                    return true;*/

                //Next find it in the server data
                if (serverProps.TryGetValue(key, out token))
                    return true;

                return false;
            }
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
            lock (mutex)
            {
                if (pendingChanges.ContainsKey(key))
                    pendingChanges[key] = token;
                else
                    pendingChanges.Add(key, token);
            }
        }
    }
}
