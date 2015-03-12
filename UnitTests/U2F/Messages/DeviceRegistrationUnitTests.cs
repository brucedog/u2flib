using Microsoft.VisualStudio.TestTools.UnitTesting;
using u2flib.Data;

namespace UnitTests.Messages
{
    [TestClass]
    public class DeviceRegistrationUnitTests
    {
        private const string JsonData ="{\"KeyHandle\":\"KlUt/bdHftZf2EEz+GGWAQsiFbV9p10xW3uej+LjklpgGVUbq2HRZZFlnLrwC0lQ96v+ZmDi4Ab3aGi3ctcMJQ==\"," +
                                       "\"PublicKey\":\"BLF0vEnHyiVLcNLlwgfO6c8XSCDr136jxlUIwm2lG2V8HMa5UvhiFpeTZILaCm09OCalkJXa9s18A+LmA4XS9tk=\"," +
                                       "\"AttestationCert\":\"MIIBPDCB5KADAgECAgpHkBKAABFVlXNSMAoGCCqGSM49BAMCMBcxFTATBgNVBAMTDEdudWJieSBQaWxvdDAeFw0xMjA4MTQxODI5MzJaFw0xMzA4MTQxODI5MzJaMDExLzAtBgNVBAMTJlBpbG90R251YmJ5LTAuNC4xLTQ3OTAxMjgwMDAxMTU1OTU3MzUyMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEjWF+ZclQjmS8xWc6yCpnmdo8FEZoLCWMRj//31jf0vo+bDeLU9eVxKTf+0GZ7deGLyOrrwIDtLiRG6BWmZThATAKBggqhkjOPQQDAgNHADBEAiBgzbYGHpwiJi0arB2W2McIKbI2ZTHdomiDLLg2vNMN+gIgYxsUWfCeYzAFVyLI2Jt/SIg7kIm4jWDR2XlZArMEEN8=\"," +
                                       "\"Counter\":0}";
 
        [TestMethod]
        public void DeviceRegistration_FromJson()
        {
            DeviceRegistration deviceRegistration = DeviceRegistration.FromJson<DeviceRegistration>(JsonData);

            Assert.IsNotNull(deviceRegistration);
            Assert.IsNotNull(deviceRegistration.KeyHandle);
            Assert.IsNotNull(deviceRegistration.PublicKey);
            Assert.IsNotNull(deviceRegistration.GetAttestationCertificate());
            Assert.IsNotNull(deviceRegistration.ToJsonWithOutAttestionCert());
            Assert.IsNotNull(deviceRegistration.ToJson());
            Assert.IsTrue(deviceRegistration.PublicKey.Length > 0);
            Assert.IsTrue(deviceRegistration.KeyHandle.Length > 0);
            Assert.IsTrue(deviceRegistration.GetHashCode() != 0);
        }
    }
}