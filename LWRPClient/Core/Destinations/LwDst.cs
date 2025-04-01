using LWRPClient.Entities;
using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.Destinations
{
    class LwDst : LwIoItem, ILWRPDestination
    {
        public LwDst(LWRPConnection conn, int index) : base(conn, index)
        {
        }

        protected override string MessageName => "DST";

        public string Name
        {
            get => ReadPropertyString("NAME");
            set => SetProperty("NAME", new LWRPToken(true, value));
        }

        public string Address
        {
            get => ReadPropertyString("ADDR");
            set => SetProperty("ADDR", new LWRPToken(true, value));
        }

        public int ChannelCount
        {
            get => ReadPropertyInt("NCHN", 2);
            set => SetProperty("NCHN", new LWRPToken(false, value.ToString()));
        }

        public LwChannel Channel
        {
            get => LwChannel.FromIpAddress(Address);
            set
            {
                if (value == null || value.Type == LwChannelType.INVALID)
                    Address = "";
                else
                    Address = value.ToIpAddress().ToString();
            }
        }
    }
}
