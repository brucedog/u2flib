using System.Collections.ObjectModel;
using System.Linq;
using BaseLibrary;
using DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services;
using u2flib.Data.Messages;
using u2flib.Util;
using DeviceRegistration = u2flib.Data.DeviceRegistration;

namespace UnitTests
{
    [TestClass]
    public class MemeberShipServiceUnitTests
    {
        private Mock<IUserRepository> _userRepository;
        private RegisterResponse _registerResponse;
        private DeviceRegistration _deviceRegistration;
        private AuthenticateResponse _authenticateResponse;
        private User _user;
        private StartedRegistration _startedRegistration;
        private StartedAuthentication _startedAuthentication;

        [TestInitialize]
        public void SetUp()
        {
            CreateResponses();
            _userRepository = new Mock<IUserRepository>();
            _user = new User
                {
                    Name = "test",
                    Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                    DeviceRegistrations = new Collection<Device>
                        {
                            new Device
                                {
                                    KeyHandle = _deviceRegistration.KeyHandle,
                                    PublicKey = _deviceRegistration.PublicKey,
                                    AttestationCert = _deviceRegistration.AttestationCert,
                                    Counter = _deviceRegistration.Counter
                                }
                        },
                    AuthenticationRequest = new AuthenticationRequest
                        {
                            AppId = "test",
                            KeyHandle = _authenticateResponse.KeyHandle,
                            Challenge = _authenticateResponse.GetClientData().Challenge
                        }
                };
        }

        [TestMethod]
        public void MemeberShipService_ConstructsProperly()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            Assert.IsNotNull(memeberShipService);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeSucess()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(_user);
            _userRepository.Setup(
                e =>
                e.SaveUserAuthenticationRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<string>()));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNotNull(result);
            _userRepository.Verify(
                e =>
                e.SaveUserAuthenticationRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoDeviceFound()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(new User { DeviceRegistrations = new Collection<Device>() });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNull(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser("test"));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNull(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoUsername()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerChallenge("");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistration()
        {
            _userRepository.Setup(e => e.AddUser(It.Is<string>(p => p == "test"), It.IsAny<string>()));
            _userRepository.Setup(e => e.AddAuthenticationRequest(It.Is<string>(p => p == "test"), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerRegistration("test", "password");

            Assert.IsNotNull(result);
            _userRepository.Verify(e => e.AddAuthenticationRequest(It.Is<string>(p => p == "test"), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistrationNoPassword()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerRegistration("test", "");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistrationNoUserName()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.GenerateServerRegistration("", "password");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationSucess()
        {
            Device device = _user.DeviceRegistrations.First();
            _user.AuthenticationRequest.Challenge = _startedRegistration.Challenge;
            _user.AuthenticationRequest.AppId = _startedRegistration.AppId;
            
            _userRepository.Setup(e => e.FindUser("test")).Returns(_user);
            _userRepository.Setup(e => e.RemoveUsersAuthenticationRequest("test"));
            _userRepository.Setup(e => e.AddDeviceRegistration("Test", device.AttestationCert, device.Counter, device.KeyHandle, device.PublicKey));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.CompleteRegistration("test", _registerResponse.ToJson());

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.RemoveUsersAuthenticationRequest("test"), Times.Once);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser("test"));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationUserFoundWithNoAuthenticationRequest()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(new User());
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoDeviceResponse()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.CompleteRegistration("test", "");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoUserName()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.CompleteRegistration("", "nothing");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegistered()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User
                {
                    DeviceRegistrations = new Collection<Device>
                        {
                            new Device()
                        }
                });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsUserRegistered("test");

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegisteredNoUserName()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User
            {
                DeviceRegistrations = new Collection<Device>
                        {
                            new Device()
                        }
            });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsUserRegistered("");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Never);
        }

        [TestMethod]
        public void MemeberShipService_IsUserRegisteredNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test")));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsUserRegistered("test");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoUserName()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoDeviceResponse()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoUserFound()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "test")));

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoDeviceFound()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "test"))).Returns(new User { DeviceRegistrations = new Collection<Device>() });

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoAuthenticationRequestFound()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "test"))).Returns(new User
            {
                DeviceRegistrations = new Collection<Device>
                            {
                                new Device
                                    {
                                        KeyHandle = _deviceRegistration.KeyHandle,
                                        PublicKey = _deviceRegistration.PublicKey,
                                        AttestationCert = _deviceRegistration.AttestationCert,
                                        Counter = _deviceRegistration.Counter
                                    }
                            }
            });

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUser()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(
                new User
                    {
                        Name = "test",
                        Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                        DeviceRegistrations = new Collection<Device>
                            {
                                new Device
                                    {
                                        KeyHandle = _deviceRegistration.KeyHandle,
                                        PublicKey = _deviceRegistration.PublicKey,
                                        AttestationCert = _deviceRegistration.AttestationCert,
                                        Counter = _deviceRegistration.Counter
                                    }
                            },
                        AuthenticationRequest = new AuthenticationRequest
                            {
                                AppId = _startedAuthentication.AppId,
                                KeyHandle = _startedAuthentication.KeyHandle,
                                Challenge = _startedAuthentication.Challenge
                            }
                    });
            _userRepository.Setup(e => e.RemoveUsersAuthenticationRequest(It.Is<string>(p => p == "test")));
            _userRepository.Setup(e => e.UpdateDeviceCounter(It.Is<string>(p => p == "test"), It.IsAny<byte[]>(), It.IsAny<uint>()));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
            _userRepository.Verify(e => e.RemoveUsersAuthenticationRequest(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordNullPassword()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsValidUserNameAndPassword("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPassword()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User { Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=" });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordNoUser()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test")));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemeberShipService_IsValidUserNameAndPasswordBadPassword()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User{Password = "notSame"});
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository.Object);

            var result = memeberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser(It.Is<string>(p => p == "test")), Times.Once);
        }

        private void CreateResponses()
        {
            _startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);

            _registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64,
                                                                     TestConts.CLIENT_DATA_REGISTER_BASE64);
            
            _deviceRegistration = new DeviceRegistration(
                TestConts.KEY_HANDLE_BASE64_BYTE, 
                TestConts.USER_PUBLIC_KEY_AUTHENTICATE_HEX,
                Utils.Base64StringToByteArray(TestConts.ATTESTATION_CERTIFICATE), 
                0);

            _authenticateResponse = new AuthenticateResponse(
                TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                TestConts.SIGN_RESPONSE_DATA_BASE64,
                TestConts.KEY_HANDLE_BASE64);

            _startedAuthentication = new StartedAuthentication(
                    TestConts.SERVER_CHALLENGE_SIGN_BASE64,
                    TestConts.APP_SIGN_ID,
                    TestConts.KEY_HANDLE_BASE64);
        }
    }
}
