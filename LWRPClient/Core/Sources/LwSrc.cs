using LWRPClient.Layer1;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWRPClient.Core.Sources
{
    class LwSrc : LwIoItem, ILWRPSource
    {
        public LwSrc(LWRPConnection conn, int index) : base(conn, index)
        {

        }

        protected override string MessageName => "SRC";

        public string PrimarySourceName
        {
            get => ReadPropertyString("PSNM");
            set => SetProperty("PSNM", new LWRPToken(true, value));
        }

        public bool RtpStreamEnabled
        {
            get => ReadPropertyBool("RTPE");
            set => SetProperty("RTPE", new LWRPToken(false, value ? "1" : "0"));
        }

        public string RtpStreamAddress
        {
            get => ReadPropertyString("RTPA");
            set => SetProperty("RTPA", new LWRPToken(true, value));
        }

        public int ChannelCount
        {
            get => ReadPropertyInt("NCHN", 2);
            set => SetProperty("NCHN", new LWRPToken(false, value.ToString()));
        }

        public int Gain
        {
            get => ReadPropertyInt("INGN");
            set => SetProperty("INGN", new LWRPToken(false, value.ToString()));
        }

        public string LcdLabel
        {
            get => ReadPropertyString("LABL");
            set => SetProperty("LABL", new LWRPToken(true, value));
        }
    }
}
