using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Util;

namespace UnitTests
{
    [TestClass]
    public class UtilsUnitTests
    {
        [TestMethod]
        public void ByteArrayToBase64String_Test()
        {
            byte[] testStringByteArray = Utils.GetBytes(TestConts.SERVER_CHALLENGE_REGISTER_BASE64);

            string result = Utils.ByteArrayToBase64String(testStringByteArray);

            Assert.IsFalse(result.Contains("+"));
            Assert.IsFalse(result.Contains("/"));
            Assert.IsFalse(result.Contains("="));
        }

        [TestMethod]
        public void Base64StringToByteArray_Test()
        {
            byte[] result = Utils.Base64StringToByteArray(TestConts.SERVER_CHALLENGE_REGISTER_BASE64);

            Assert.IsTrue(result.Length > 0);
        }
    }
}