using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib;
using u2flib.Data.Messages;
using u2flib.Util;

namespace UnitTests.Messages
{
    [TestClass]
    public class RawRegisterResponseUnitTests
    {
        [TestMethod]
        public void RawRegisterResponse_FromBase64()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);

            Assert.IsNotNull(rawAuthenticateResponse);
            Assert.IsNotNull(rawAuthenticateResponse.CreateDevice());
            Assert.IsTrue(rawAuthenticateResponse.GetHashCode() != 0);
        }

        [TestMethod]
        public void RawRegisterResponse_Equals()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RawRegisterResponse rawAuthenticateResponse1 = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);

            Assert.IsTrue(rawAuthenticateResponse.Equals(rawAuthenticateResponse1));
        }

        [TestMethod]
        public void RawRegisterResponse_PackBytesToSign()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);

            byte[] packedBytes = rawAuthenticateResponse.PackBytesToSign(
                U2F.Crypto.Hash("appid"),
                Utils.Base64StringToByteArray(TestConts.CLIENT_DATA_REGISTER),
                TestConts.KEY_HANDLE_BASE64_BYTE,
                TestConts.USER_PUBLIC_KEY_AUTHENTICATE_HEX);

            Assert.IsNotNull(packedBytes);
            Assert.IsTrue(packedBytes.Length > 0);
        }
    }
}