using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Data.Messages;

namespace UnitTests.Messages
{
    [TestClass]
    public class StartedAuthenticationUnitTests
    {
        private const string JsonData = "{\"Version\":\"U2F_V2\",\"KeyHandle\":\"KlUt/bdHftZf2EEz+GGWAQsiFbV9p10xW3uej+LjklpgGVUbq2HRZZFlnLrwC0lQ96v+ZmDi4Ab3aGi3ctcMJQ==\",\"Challenge\":\"opsXqUifDriAAmWclinfbS0e-USY0CgyJHe_Otd7z8o\",\"AppId\":\"http://example.com\"}";

        [TestMethod]
        public void StartedAuthentication_ConstructsProperly()
        {
            StartedAuthentication startedAuthentication =
                new StartedAuthentication(TestConts.SERVER_CHALLENGE_SIGN_BASE64, TestConts.APP_ID_ENROLL,
                                          TestConts.KEY_HANDLE_BASE64);

            Assert.IsNotNull(startedAuthentication);
            Assert.IsNotNull(startedAuthentication.Version);
            Assert.IsNotNull(startedAuthentication.ToJson());
            Assert.IsTrue(startedAuthentication.GetHashCode() != 0);
            Assert.AreEqual(TestConts.APP_ID_ENROLL, startedAuthentication.AppId);
            Assert.AreEqual(TestConts.KEY_HANDLE_BASE64, startedAuthentication.KeyHandle);
            Assert.AreEqual(TestConts.SERVER_CHALLENGE_SIGN_BASE64, startedAuthentication.Challenge);
        }

        [TestMethod]
        public void StartedAuthentication_FromJson()
        {
            StartedAuthentication startedAuthentication = StartedAuthentication.FromJson(JsonData);

            Assert.IsNotNull(startedAuthentication);
            Assert.IsNotNull(startedAuthentication.Version);
            Assert.IsNotNull(startedAuthentication.ToJson());
            Assert.IsTrue(startedAuthentication.GetHashCode() != 0);
            Assert.AreEqual(TestConts.APP_ID_ENROLL, startedAuthentication.AppId);
            Assert.AreEqual(TestConts.KEY_HANDLE_BASE64, startedAuthentication.KeyHandle);
            Assert.AreEqual(TestConts.SERVER_CHALLENGE_SIGN_BASE64, startedAuthentication.Challenge);
        }

        [TestMethod]
        public void StartedAuthentication_Equals()
        {
            StartedAuthentication sameStartedAuthentication =
                new StartedAuthentication(TestConts.SERVER_CHALLENGE_SIGN_BASE64, TestConts.APP_ID_ENROLL,
                                          TestConts.KEY_HANDLE_BASE64);
            StartedAuthentication startedAuthentication = StartedAuthentication.FromJson(JsonData);

            Assert.IsTrue(startedAuthentication.Equals(sameStartedAuthentication));
        }
    }
}