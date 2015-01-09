using System.Collections.ObjectModel;
using DemoU2FSite.Repository;
using DemoU2FSite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace UnitTests
{
    [TestClass]
    public class MemeberShipServiceUnitTests
    {
        private IUserRepository _userRepository;
        private string RegisterResponseFromJson = "{\"registrationData\":\"WsSzzqUMR24NpaOPmQfiTX8nUMssX0XwruXEXWXq7iDQ106q196wMrNPIFLIclOPHkPiGuXaPCn77DQDiWE5xLb2QNhW2P_yJ4HBUaBqGl9svpLIN5zMYWnwgdDkeOzKfAu0f40QmMpBW2bdX5pRRK7frMKBDLUI0aWD8EbCVq4GnrgwggIbMIIBBaADAgECAgR1o_Z1MXsGCSqGSIb3DQEBCzAuMSwwKgYDVQQDEyNZdWJpY28gVTJGIFJvb3QgQ0EgU2VyaWFsIDQ1NzIwMDYzMTAgFw0xNDA4MDEwMDAwMDBaGA8yMDUwMDkwAwPJAwMFowKjEoMCYGA1UEAwwfWXViaWNvIFUyRiBFRSBTZXJpYWwgMTk3MzY3OTczMzBFMBMGByqGSM49AgEGCCqGSM49AwEHA0IABBmjfkNqa2mXzVh2ZxuES5coCvvENxDMDLmfd-0ACG0Fu7wR4ZTjKd9KAuidySpfona5csGmlM0Te_Zu35h_vvujEjAQMA4GCisGAQQBgsQKAQIEADALBgkqhkiG9w0BAQsDggEBAb0tuI0-CzSxIp4cAlyD6UyT4cKyJZGVhWdtPgj_mWepT3Tu9jXtdgA5F3jfZtTc2eGxuS-PPvqRAkZd40AXgM8A0YaXPwlT4s0RUTY9Y8aAQzQZeAHuZk3lKKd_PPCg5077dzdt90lC5eVTEduj6cOnHEqnOr2Cv75FuiQXX7QkGQxtoD-otgvhZ2Fjk29o7Iy9ik7ewHGXOfoVw_ruGWi0YfXBTuqEJ6H666vvMN4BZWHtzhC0k5ceQslB9Xdntky-GQgDqHGkBf32GKwAFT9JJrkO2BfsB-wfBrTiHr0AABYNTNKTceA5dtR3UVpI492VUWQbY3VwWUUfKTI7fM4wRAIgJsly62P1LFpNzXMU4Dq8aedEDXBFaZG0-J3kumgmK3cCIAF6S-1ZratDwLoCs8d4wgSAaoLU8yrKOmQ0La1riaxo\",\"challenge\":\"dQoKOvQ1Oq-oKHPawoaQrBJ-AfRu1o1l6v63Rz6CIq4\",\"version\":\"U2F_V2\",\"appId\":\"http://localhost:52701\",\"clientData\":\"eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZmluaXNoRW5yb2xsbWVudCIsImNoYWxsZW5nZSI6ImBb0hJdlExT3Etb0tIUGF3c2FRckJKLUFmUnAxbzFsNnY2M1J6NkhScTQiLCJvcmlnaW4iOiJodBJwOi8vbG9jCIxob3N0OjUyNzAxIiwiY2lkX3B1YmtleSI6IiJ9\"}";
        private string AuthenticateResponseFromJson = "{\"keyHandle\":\"1jzoZLnuWs781OqcyS7dtCjTXkndq2Rm9803B_0EqjDyeJDVjN7ZZeWAz77347TsoRqKIxGLgux_20hs-PiRMQ\",\"clientData\":\"eyJ0eXAiOiJuYXZpZ2F0b3IuaWQuZ2V0QXNzZ1J0aW9uIiwiY2hhbGxlbmdlIjoiOTNXUDNzSGZmLTY4X3VVaDJCWkpCMmVZMXg2cERkVW5sMVNOTFVLeUQ1NCIsIm9yaWdVTiI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI3MDEiLCJjaWRfcHVia2V5IjoiIn0\",\"signatureData\":\"AQNDACAwRAIgGZRu0ChUxDAXjtBpiQDgQdkBiI5RA5mCZswWuqBVT-sCIGksC1M8bVho0g2K06ikPOnmUmKR15MCxepEHGpIpPJE\"}";
        
        [TestInitialize]
        public void SetUp()
        {
            _userRepository = MockRepository.GenerateMock<IUserRepository>();
        }

        [TestMethod]
        public void MemeberShipService_ConstructsProperly()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            Assert.IsNotNull(memeberShipService);
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegistered()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(new User
                {
                    DeviceRegistrations = new Collection<DeviceRegistration>
                        {
                            new DeviceRegistration()
                        }
                });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsUserRegistered("test");

            Assert.IsTrue(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegisteredNoUserName()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(new User
            {
                DeviceRegistrations = new Collection<DeviceRegistration>
                        {
                            new DeviceRegistration()
                        }
            });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsUserRegistered("");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegisteredNoUserFound()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(null);
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsUserRegistered("test");

            Assert.IsFalse(result);
            _userRepository.VerifyAllExpectations();
        }

        [Ignore]
        [TestMethod]
        public void MemeberShipService_AuthenticateUser()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(
                new User
                {
                    Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                    DeviceRegistrations = new Collection<DeviceRegistration>
                                          {
                                              new DeviceRegistration()
                                          }
                });
            _userRepository.Expect(e => e.UpdateDeviceCounter(Arg<string>.Is.Equal("test"), Arg<byte[]>.Is.Anything, Arg<uint>.Is.Anything));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", AuthenticateResponseFromJson);

            Assert.IsTrue(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordNullPassword()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsValidUserNameAndPassword("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPassword()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(new User { Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=" });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsTrue(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordNoUser()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(null);
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsFalse(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordBadPassword()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(new User{Password = "notSame"});
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsFalse(result);
            _userRepository.VerifyAllExpectations();
        }
    }
}
