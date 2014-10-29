using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib;
using u2flib.Data;
using u2flib.Data.Messages;

namespace UnitTests
{
    [TestClass]
    public class U2FUnitTests
    {
        [TestMethod]
        public void U2F_StartRegistration()
        {
            var results = U2F.StartRegistration(TestConts.APP_ID_ENROLL);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.Challenge);
            Assert.IsNotNull(results.Version);
            Assert.AreEqual(results.AppId, TestConts.APP_ID_ENROLL);
        }

        [TestMethod]
        public void U2F_FinishAuthentication()
        {
            StartedAuthentication startedAuthentication =
                new StartedAuthentication(TestConts.SERVER_CHALLENGE_SIGN_BASE64, TestConts.APP_ID_ENROLL,
                                          TestConts.KEY_HANDLE_BASE64);
            AuthenticateResponse authenticateResponse = new AuthenticateResponse(TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                                                                                 TestConts.SIGN_RESPONSE_DATA_BASE64,
                                                                                 TestConts.KEY_HANDLE_BASE64);
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);
            DeviceRegistration deviceRegistration = rawAuthenticateResponse.CreateDevice();
            int orginalValue = deviceRegistration.Counter;

            U2F.FinishAuthentication(startedAuthentication, authenticateResponse, deviceRegistration);

            Assert.IsTrue(deviceRegistration.Counter != 0);
            Assert.AreNotEqual(orginalValue, deviceRegistration.Counter);
        }

        [TestMethod]
        public void U2F_FinishRegistration()
        {
            StartedRegistration startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);

            var results = U2F.FinishRegistration(startedRegistration, registerResponse, TestConts.TRUSTED_DOMAINS);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.KeyHandle);
            Assert.IsNotNull(results.PublicKey);
            Assert.IsNotNull(results.GetAttestationCertificate());
        }

        [TestMethod]
        public void U2F_FinishRegistrationNoFacets()
        {
            StartedRegistration startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);

            var results = U2F.FinishRegistration(startedRegistration, registerResponse);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.KeyHandle);
            Assert.IsNotNull(results.PublicKey);
            Assert.IsNotNull(results.GetAttestationCertificate());
        }

        [TestMethod]
        public void U2F_StartAuthentication()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RawRegisterResponse rawAuthenticateResponse = RawRegisterResponse.FromBase64(registerResponse.RegistrationData);
            DeviceRegistration deviceRegistration = rawAuthenticateResponse.CreateDevice();

            var results = U2F.StartAuthentication(TestConts.APP_ID_ENROLL, deviceRegistration);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.AppId);
            Assert.IsNotNull(results.Challenge);
            Assert.IsNotNull(results.KeyHandle);
            Assert.IsNotNull(results.Version);
        }
    }
}