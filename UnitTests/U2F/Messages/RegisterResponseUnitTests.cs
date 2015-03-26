using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Data.Messages;

namespace UnitTests.Messages
{
    [TestClass]
    public class RegisterResponseUnitTests
    {
        private const string JsonData = "{\"RegistrationData\":\"BQSxdLxJx8olS3DS5cIHzunPF0gg69d+o8ZVCMJtpRtlfBzGuVL4YhaXk2SC2gptPTgmpZCV2vbNfAPi5gOF0vbZQCpVLf23R37WX9hBM/hhlgELIhW1faddMVt7no/i45JaYBlVG6th0WWRZZy68AtJUPer/mZg4uAG92hot3LXDCUwggE8MIHkoAMCAQICCkeQEoAAEVWVc1IwCgYIKoZIzj0EAwIwFzEVMBMGA1UEAxMMR251YmJ5IFBpbG90MB4XDTEyMDgxNDE4MjkzMloXDTEzMDgxNDE4MjkzMlowMTEvMC0GA1UEAxMmUGlsb3RHbnViYnktMC40LjEtNDc5MDEyODAwMDExNTU5NTczNTIwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAASNYX5lyVCOZLzFZzrIKmeZ2jwURmgsJYxGP//fWN/S+j5sN4tT15XEpN/7QZnt14YvI6uvAgO0uJEboFaZlOEBMAoGCCqGSM49BAMCA0cAMEQCIGDNtgYenCImLRqsHZbYxwgpsjZlMd2iaIMsuDa80w36AiBjGxRZ8J5jMAVXIsjYm39IiDuQibiNYNHZeVkCswQQ3zBFAiAUcYmbzDmH5i6CAsmznDPBkDP3NANS26gPyrAX25Iw5AIhAIJnfWc9iRkzreb2F+Xb3i4kfnBCP9WteASm09OWHvhx\",\"ClientData\":\"eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZmluaXNoRW5yb2xsbWVudCIsImNoYWxsZW5nZSI6InZxclM2V1hEZTFKVXM1X2MzaTQtTGtLSUhSci0zWFZiM2F6dUE1VGlmSG8iLCJjaWRfcHVia2V5Ijp7Imt0eSI6IkVDIiwiY3J2IjoiUC0yNTYiLCJ4IjoiSHpRd2xmWFg3UTRTNU10Q0NuWlVOQnczUk16UE85dE95V2pCcVJsNHRKOCIsInkiOiJYVmd1R0ZMSVp4MWZYZzN3TnFmZGJuNzVoaTQtXzctQnhoTWxqdzQySHQ0In0sIm9yaWdpbiI6Imh0dHA6Ly9leGFtcGxlLmNvbSJ9\"}";
        
        [TestMethod]
        public void RegisterResponse_ConstructsProperly()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);     

            Assert.IsNotNull(registerResponse);
            Assert.IsNotNull(registerResponse.GetClientData());
            Assert.IsNotNull(registerResponse.ToJson());
            Assert.IsTrue(registerResponse.GetHashCode() != 0);
            Assert.AreEqual(JsonData, registerResponse.ToJson());
            Assert.AreEqual(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, registerResponse.RegistrationData);
            Assert.AreEqual(TestConts.CLIENT_DATA_REGISTER_BASE64, registerResponse.ClientData);
        }

        [TestMethod]
        public void RegisterResponse_FromJson()
        {
            RegisterResponse registerResponse = RegisterResponse.FromJson<RegisterResponse>(JsonData);

            Assert.IsNotNull(registerResponse);
            Assert.IsNotNull(registerResponse.GetClientData());
            Assert.IsNotNull(registerResponse.ToJson());
            Assert.IsTrue(registerResponse.GetHashCode() != 0);
            Assert.AreEqual(JsonData, registerResponse.ToJson());
            Assert.AreEqual(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, registerResponse.RegistrationData);
            Assert.AreEqual(TestConts.CLIENT_DATA_REGISTER_BASE64, registerResponse.ClientData);
        }

        [TestMethod]
        public void RegisterResponse_Equals()
        {
            RegisterResponse registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64, TestConts.CLIENT_DATA_REGISTER_BASE64);
            RegisterResponse sameRegisterResponse = RegisterResponse.FromJson<RegisterResponse>(JsonData);

            Assert.IsTrue(sameRegisterResponse.Equals(registerResponse));
        }
    }
}