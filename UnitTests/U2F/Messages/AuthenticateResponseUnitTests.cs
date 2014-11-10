using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Data.Messages;

namespace UnitTests.Messages
{
    [TestClass]
    public class AuthenticateResponseUnitTests
    {
        private const string JsonData = "{\"SignatureData\":\"AQAAAAEwRAIgS18M0XU0zt2MNO4JVw71QqNT30Q2AwzkPUBt6HC4R3gCICZ7uZj6ybcmbrYOfLC16r39W6lhT1PHsiJy7BAEepI/\",\"ClientData\":" + "\"eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZ2V0QXNzZXJ0aW9uIiwiY2hhbGxlbmdlIjoib3BzWHFVaWZEcmlBQW1XY2xpbmZiUzBlLVVTWTBDZ3lKSGVfT3RkN3o4byIsImNpZF9wdWJrZXkiOnsia3R5IjoiRUMiLCJjcnYiOiJQLTI1NiIsIngiOiJIelF3bGZYWDdRNFM1TXRDQ25aVU5CdzNSTXpQTzl0T3lXakJxUmw0dEo4IiwieSI6IlhWZ3VHRkxJWngxZlhnM3dOcWZkYm43NWhpNC1fNy1CeGhNbGp3NDJIdDQifSwib3JpZ2luIjoiaHR0cDovL2V4YW1wbGUuY29tIn0=\"," + "\"KeyHandle\":\"KlUt/bdHftZf2EEz+GGWAQsiFbV9p10xW3uej+LjklpgGVUbq2HRZZFlnLrwC0lQ96v+ZmDi4Ab3aGi3ctcMJQ==\"}";

        [TestMethod]
        public void AuthenticateResponse_ConstructsProperly()
        {
            AuthenticateResponse authenticateResponse = new AuthenticateResponse(TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                                                                                 TestConts.SIGN_RESPONSE_DATA_BASE64,
                                                                                 TestConts.KEY_HANDLE_BASE64);

            Assert.IsNotNull(authenticateResponse);
            Assert.IsNotNull(authenticateResponse.ToJson());
            Assert.IsNotNull(authenticateResponse.GetClientData());
            Assert.IsTrue(authenticateResponse.GetHashCode() != 0);
            Assert.AreEqual(JsonData, authenticateResponse.ToJson());
            Assert.AreEqual(TestConts.SIGN_RESPONSE_DATA_BASE64, authenticateResponse.SignatureData);
            Assert.AreEqual(TestConts.KEY_HANDLE_BASE64, authenticateResponse.KeyHandle);
        }

        [TestMethod]
        public void AuthenticateResponse_FromJson()
        {
            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(JsonData);

            Assert.IsNotNull(authenticateResponse);
            Assert.AreEqual(authenticateResponse.GetType(), typeof(AuthenticateResponse));
        }

        [TestMethod]
        public void AuthenticateResponse_Equals()
        {
            AuthenticateResponse authenticateResponse = AuthenticateResponse.FromJson(JsonData);
            AuthenticateResponse sameAuthenticateResponse = new AuthenticateResponse(
                TestConts.CLIENT_DATA_AUTHENTICATE_BASE64, 
                TestConts.SIGN_RESPONSE_DATA_BASE64,
                TestConts.KEY_HANDLE_BASE64);

            Assert.IsNotNull(authenticateResponse);
            Assert.IsNotNull(sameAuthenticateResponse);
            Assert.IsTrue(authenticateResponse.Equals(sameAuthenticateResponse));
        }
    }
}