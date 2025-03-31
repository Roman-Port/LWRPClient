using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Layer1
{
    public class LWRPToken
    {
        public LWRPToken(bool quotes, string content = "")
        {
            this.quotes = quotes;
            Content = content;
        }

        private string content;
        private readonly bool quotes;

        public string Content
        {
            get => content;
            set
            {
                if (value == null)
                    throw new Exception("Value cannot be null.");
                if (value.Contains(" ") && !quotes)
                    throw new Exception("Value contains spaces when no quotes will be included.");
                content = value;
            }
        }

        public string Serialize()
        {
            //Escape content
            string escaped = content.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");

            //Add quotes as needed
            if (quotes)
                return "\"" + escaped + "\"";
            return escaped;
        }

        /// <summary>
        /// Reads a token from a character stream. Reads until end of array, \r, \n, a space/colon outside of a substring. Returns null if end of stream.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static LWRPToken ReadToken(char[] chars, ref int index)
        {
            //Skip spaces
            while (index < chars.Length && chars[index] == ' ')
                index++;

            //If end, return null
            if (index >= chars.Length)
                return null;

            //If this is a quote, treat as a substring
            string text = "";
            bool inSubstring = chars[index] == '\"';
            if (inSubstring)
            {
                //In a substring; Read until the next unescaped quote
                index++; // Skip quote
                bool escaped = false;
                while (index < chars.Length && chars[index] != '\n')
                {
                    //Disregard \r
                    if (chars[index] == '\r')
                        continue;

                    //Process
                    if (escaped)
                    {
                        //Switch on the type of escape
                        switch (chars[index])
                        {
                            case 'r':
                                text += "\r";
                                break;
                            case 'n':
                                text += "\n";
                                break;
                            case '\"':
                                text += "\"";
                                break;
                            case '\\':
                                text += "\\";
                                break;
                            default:
                                throw new Exception("Unknown escape code: " + chars[index]);
                        }
                        escaped = false;
                    }
                    else if (chars[index] == '\\')
                    {
                        //Starting escape; discard this character
                        escaped = true;
                    }
                    else if (chars[index] == '\"')
                    {
                        //End substring
                        index++;
                        break;
                    }
                    else
                    {
                        //Regular character
                        text += chars[index];
                    }
                    index++;
                }

            }
            else
            {
                //This is not a substring which makes things easier. Simply read to a colon, space, or \r\n
                while (index < chars.Length && chars[index] != ' ' && chars[index] != ':' && chars[index] != '\n')
                {
                    //Disregard \r
                    if (chars[index] != '\r')
                        text += chars[index];
                    index++;
                }
            }

            //Return token
            return new LWRPToken(inSubstring, text);
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}
