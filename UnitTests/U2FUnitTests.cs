using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib;
using u2flib.Data.Messages;

namespace UnitTests
{
    [TestClass]
    public class U2FUnitTests
    {
        // TODO add more unit tests
        [TestMethod]
        public void TestMethod1()
        {
            StartedRegistration startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);

            var results = U2F.FinishAuthentication(startedRegistration, new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64), TestConts.TRUSTED_DOMAINS);

            Assert.IsNotNull(results);
            Assert.IsNotNull(results.GetPublicKey());
            Assert.IsNotNull(results.GetAttestationCertificate());
            Assert.IsNotNull(results.GetKeyHandle());
        }
    }
}
