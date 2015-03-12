using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib;
using u2flib.Data.Messages;

namespace UnitTests.Messages
{
    [TestClass]
    public class RawAuthenticateResponseUnitTests
    {
        private AuthenticateResponse _authenticateResponse= null;
        private ClientData clientData;
        public const String AuthenticateTyp = "navigator.id.getAssertion";
        public const String RegisterType = "navigator.id.finishEnrollment";
        
        [TestInitialize]
        public void Setup()
        {
            StartedAuthentication startedAuthentication =
                new StartedAuthentication(TestConts.SERVER_CHALLENGE_SIGN_BASE64, TestConts.APP_ID_ENROLL,
                                          TestConts.KEY_HANDLE_BASE64);
            _authenticateResponse = new AuthenticateResponse(TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                                                                                 TestConts.SIGN_RESPONSE_DATA_BASE64,
                                                                                 TestConts.KEY_HANDLE_BASE64);

            clientData = _authenticateResponse.GetClientData();
            clientData.CheckContent(AuthenticateTyp, startedAuthentication.Challenge, null);
        }

        [TestMethod]
        public void RawAuthenticateResponse_FromBase64()
        {
            RawAuthenticateResponse rawAuthenticateResponse = RawAuthenticateResponse.FromBase64(_authenticateResponse.SignatureData);

            Assert.IsNotNull(rawAuthenticateResponse);
            Assert.IsNotNull(rawAuthenticateResponse.UserPresence);
            Assert.IsNotNull(rawAuthenticateResponse.ToString());
            Assert.IsTrue(rawAuthenticateResponse.UserPresence > 0);
            Assert.IsTrue(rawAuthenticateResponse.GetHashCode() > 0);
            Assert.IsTrue(rawAuthenticateResponse.Signature.Length > 0);
        }

        [TestMethod]
        public void RawAuthenticateResponse_PackBytesToSign()
        {
            RawAuthenticateResponse rawAuthenticateResponse = RawAuthenticateResponse.FromBase64(_authenticateResponse.SignatureData);

            byte[] signedBytes = rawAuthenticateResponse.PackBytesToSign(
               U2F.Crypto.Hash("testid"),
               rawAuthenticateResponse.UserPresence,
               rawAuthenticateResponse.Counter,
               U2F.Crypto.Hash(clientData.AsJson())
               );

            Assert.IsNotNull(signedBytes);
            Assert.IsTrue(signedBytes.Length > 0);
        }

        [TestMethod]
        public void RawAuthenticateResponse_Equals()
        {
            RawAuthenticateResponse rawAuthenticateResponse1 = RawAuthenticateResponse.FromBase64(_authenticateResponse.SignatureData);
            RawAuthenticateResponse rawAuthenticateResponse = RawAuthenticateResponse.FromBase64(_authenticateResponse.SignatureData);

            Assert.IsTrue(rawAuthenticateResponse1.Equals(rawAuthenticateResponse));
        }
    }
}