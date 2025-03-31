using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Layer1
{
    public class LWRPMessage
    {
        public LWRPMessage(string name, LWRPToken[][] arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }

        private readonly string name;
        private readonly LWRPToken[][] arguments;

        public string Name => name;
        public LWRPToken[][] Arguments => arguments;

        public string Serialize()
        {
            string result = Name;
            foreach (var seg in arguments)
            {
                result += " ";
                for (int i = 0; i < seg.Length; i++)
                {
                    if (i != 0)
                        result += ":";
                    result += seg[i].Serialize();
                }
            }
            return result;
        }

        /// <summary>
        /// Attempts to find an argument with a named key. Arguments with two or more tokens are scanned for a matching name with their first token.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryGetNamedArgument(string key, out LWRPToken[] token)
        {
            foreach (var a in arguments)
            {
                if (a.Length >= 2 && a[0].Content == key)
                {
                    token = a;
                    return true;
                }
            }
            token = null;
            return false;
        }

        /// <summary>
        /// Attempts to find an argument with a named key. Pulls the first token with it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryGetNamedArgument(string key, out LWRPToken token)
        {
            if (TryGetNamedArgument(key, out LWRPToken[] tokens) && tokens.Length >= 2)
            {
                token = tokens[1];
                return true;
            }
            token = null;
            return false;
        }

        public static LWRPMessage Deserialize(string data)
        {
            //Split to characters
            char[] c = data.ToCharArray();
            int index = 0;

            //Read the first token that is the command name
            LWRPToken cmd = LWRPToken.ReadToken(c, ref index);

            //Split to segments
            List<LWRPToken[]> segments = new List<LWRPToken[]>();
            List<LWRPToken> currentSegment = new List<LWRPToken>();
            while (index < c.Length && c[index] != '\n')
            {
                //Skip \r
                if (c[index] == '\r')
                    continue;

                //Read next token
                LWRPToken token = LWRPToken.ReadToken(c, ref index);
                if (token == null)
                    break;

                //Add to segment
                currentSegment.Add(token);

                //Detect if we need to start the next segment or not. If there is a colon, read into the same segment
                if (index >= c.Length || c[index] != ':')
                {
                    //Finish segment
                    segments.Add(currentSegment.ToArray());
                    currentSegment.Clear();
                }

                //Increment index to next
                index++;
            }

            //In the off chance there is anything remaining in the current segment, add it
            if (currentSegment.Count > 0)
                segments.Add(currentSegment.ToArray());

            //Wrap
            return new LWRPMessage(cmd.Content, segments.ToArray());
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}
