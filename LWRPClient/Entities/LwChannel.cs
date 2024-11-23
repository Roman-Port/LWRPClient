using LWRPClient.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace LWRPClient.Entities
{
    public class LwChannel
    {
        public LwChannel(LwChannelType type, ushort channel)
        {
            this.type = type;
            this.channel = channel;
        }

        public static LwChannel INVALID = new LwChannel(LwChannelType.INVALID, 0);

        private readonly LwChannelType type;
        private readonly ushort channel;

        public LwChannelType Type => type;
        public ushort Channel
        {
            get
            {
                if (type == LwChannelType.INVALID)
                    throw new InvalidLwChannelException();
                return channel;
            }
        }

        public static LwChannel FromIpAddress(IPAddress addr)
        {
            //Split into octets
            byte[] parts = addr.GetAddressBytes();

            //Check length and make sure it's a multicast address
            if (parts.Length == 4 && parts[0] == 239)
            {
                //Check the 2nd octet to see if this is to source or from source.
                LwChannelType type;
                switch (parts[1])
                {
                    case 192: type = LwChannelType.FROM_SOURCE; break;
                    case 193: type = LwChannelType.TO_SOURCE; break;
                    default: return INVALID;
                }

                //Calculate the channel by decoding the first two octets
                ushort channel = (ushort)(((ushort)parts[2] << 8) | parts[3]);

                //Wrap into new
                return new LwChannel(type, channel);
            }

            return INVALID;
        }

        public static LwChannel FromIpAddress(string addr)
        {
            //Check if null
            if (addr == null)
                return INVALID;

            //Split anything after the first space
            int space = addr.IndexOf(' ');
            if (space != -1)
                addr = addr.Substring(0, space);

            //Attempt to parse into IP Address
            if (IPAddress.TryParse(addr, out IPAddress parsed))
                return FromIpAddress(parsed);

            return INVALID;
        }

        public IPAddress ToIpAddress()
        {
            //Split into octets
            byte o1 = (byte)(channel >> 8);
            byte o2 = (byte)(channel & 0xFF);

            //Determine o0 based on the direction
            byte o0;
            switch (type)
            {
                case LwChannelType.INVALID: throw new InvalidLwChannelException();
                case LwChannelType.FROM_SOURCE: o0 = 192; break; 
                case LwChannelType.TO_SOURCE: o0 = 193; break;
                default: throw new Exception("Unknown LW channel type.");
            }

            //Wrap into multicast address
            return new IPAddress(new byte[] { 239, o0, o1, o2 });
        }

        public override bool Equals(object obj)
        {
            if (obj is LwChannel cmp)
                return cmp.type == type && cmp.channel == channel;
            return false;
        }

        public override int GetHashCode()
        {
            return (((int)type) << 16) | channel;
        }
    }
}
