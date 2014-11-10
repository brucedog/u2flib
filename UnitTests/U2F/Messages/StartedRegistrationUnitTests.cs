using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Data.Messages;

namespace UnitTests.Messages
{
    [TestClass]
    public class StartedRegistrationUnitTests
    {
        private const string JsonData = "{\"Version\":\"U2F_V2\",\"Challenge\":\"vqrS6WXDe1JUs5_c3i4-LkKIHRr-3XVb3azuA5TifHo\",\"AppId\":\"http://example.com\"}";

        [TestMethod]
        public void StartedRegistration_ConstructsProperly()
        {
            StartedRegistration startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);

            Assert.IsNotNull(startedRegistration);
            Assert.IsNotNull(startedRegistration.Version);
            Assert.IsNotNull(startedRegistration.Challenge);
            Assert.IsNotNull(startedRegistration.AppId);
            Assert.IsNotNull(startedRegistration.ToJson());
            Assert.IsTrue(startedRegistration.GetHashCode() != 0);
            Assert.AreEqual(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, startedRegistration.Challenge);
            Assert.AreEqual(TestConts.APP_ID_ENROLL, startedRegistration.AppId);
        }

        [TestMethod]
        public void StartedRegistration_FromJson()
        {
            StartedRegistration startedRegistration = StartedRegistration.FromJson(JsonData);

            Assert.IsNotNull(startedRegistration);
            Assert.IsNotNull(startedRegistration.Version);
            Assert.IsNotNull(startedRegistration.Challenge);
            Assert.IsNotNull(startedRegistration.AppId);
            Assert.IsNotNull(startedRegistration.ToJson());
            Assert.IsTrue(startedRegistration.GetHashCode() != 0);
            Assert.AreEqual(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, startedRegistration.Challenge);
            Assert.AreEqual(TestConts.APP_ID_ENROLL, startedRegistration.AppId);
        }

        [TestMethod]
        public void StartedRegistration_Equals()
        {
            StartedRegistration startedRegistration = StartedRegistration.FromJson(JsonData);
            StartedRegistration sameStartedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);

            Assert.IsTrue(startedRegistration.Equals(sameStartedRegistration));
        }
    }
}