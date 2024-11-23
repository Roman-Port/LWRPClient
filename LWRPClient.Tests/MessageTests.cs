using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LWRPClient.Tests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void ParseTests()
        {
            LWRPMessage msg = TestParseDecodeMessage("DST 2 NAME:\"I3G Net Out\\\\\" ADDR:\"239.192.1.45 <I3G NET EXTERNAL>\" OUGN:0 NCHN:2 VMOD:*");
            Assert.IsNotNull(msg);
        }

        private LWRPMessage TestParseDecodeMessage(string msg)
        {
            //Deserialize both with sudden end and \r\n
            LWRPMessage original = LWRPMessage.Deserialize(msg);
            LWRPMessage eol = LWRPMessage.Deserialize(msg + "\r\n");

            //Serialize and check if it matches the original
            Assert.AreEqual(original.Serialize(), msg);
            Assert.AreEqual(eol.Serialize(), msg);

            return original;
        }
    }
}
