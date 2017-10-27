using System;
using System.Collections.Generic;
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
    public class MemberShipServiceUnitTests
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
                                    Counter = (int) _deviceRegistration.Counter
                                }
                        },
                    AuthenticationRequest = new List<AuthenticationRequest>
                    {
                        new AuthenticationRequest
                        {
                            AppId = "test",
                            KeyHandle = _authenticateResponse.KeyHandle,
                            Challenge = _authenticateResponse.GetClientData().Challenge
                        }
                    }
                };
        }

        [TestMethod]
        public void MemberShipService_SaveBlankUserName()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var results = memberShipService.SaveNewUser("", "");

            Assert.IsFalse(results);
        }

        [TestMethod]
        public void MemberShipService_SaveBlankPassword()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var results = memberShipService.SaveNewUser("someone", "");

            Assert.IsFalse(results);
        }

        [TestMethod]
        public void MemberShipService_CannotSaveDuplicateUserName()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "someone"))).Returns(new User()).Verifiable();
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var results = memberShipService.SaveNewUser("someone", "password1");

            Assert.IsFalse(results);
            _userRepository.VerifyAll();
        }

        [TestMethod]
        public void MemberShipService_SaveUserName()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "someone"))).Returns((User) null).Verifiable();
            _userRepository.Setup(s => s.AddUser(It.Is<string>(p => p == "someone"), It.IsAny<string>())).Verifiable();
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var results = memberShipService.SaveNewUser("someone", "password1");

            Assert.IsTrue(results);
            _userRepository.VerifyAll();
        }

        [TestMethod]
        public void MemberShipService_GenerateServerChallengeSucess()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(_user);
            _userRepository.Setup(
                e =>
                e.SaveUserAuthenticationRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<string>()));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenges("test");

            Assert.IsNotNull(result);
            _userRepository.Verify(
                e =>
                e.SaveUserAuthenticationRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                                It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_GenerateServerChallengeNoDeviceFound()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(new User { DeviceRegistrations = new Collection<Device>() });
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenges("test");

            Assert.IsNull(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_GenerateServerChallengeNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser("test"));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenges("test");

            Assert.IsNull(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_GenerateServerChallengeNoUsername()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenges("");

            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void MemberShipService_GenerateServerRegistration()
        {
            _userRepository.Setup(e => e.SaveUserAuthenticationRequest(It.Is<string>(p => p == "test"), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenge("test");

            Assert.IsNotNull(result);
            _userRepository.Verify(e => e.SaveUserAuthenticationRequest(It.Is<string>(p => p == "test"), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_GenerateServerRegistrationForUser()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.GenerateServerChallenge("test");

            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void MemberShipService_CompleteRegistrationSucess()
        {
            Device device = _user.DeviceRegistrations.First();
            _user.AuthenticationRequest.First().Challenge = _startedRegistration.Challenge;
            _user.AuthenticationRequest.First().AppId = _startedRegistration.AppId;
            
            _userRepository.Setup(e => e.FindUser("test")).Returns(_user);
            _userRepository.Setup(e => e.RemoveUsersAuthenticationRequests("test"));
            _userRepository.Setup(e => e.AddDeviceRegistration("Test", device.AttestationCert, Convert.ToUInt32(device.Counter), device.KeyHandle, device.PublicKey));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.CompleteRegistration("test", _registerResponse.ToJson());

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.RemoveUsersAuthenticationRequests("test"), Times.Once);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_CompleteRegistrationNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser("test"));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_CompleteRegistrationUserFoundWithNoAuthenticationRequest()
        {
            _userRepository.Setup(e => e.FindUser("test")).Returns(new User());
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_CompleteRegistrationNoDeviceResponse()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.CompleteRegistration("test", "");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemberShipService_CompleteRegistrationNoUserName()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.CompleteRegistration("", "nothing");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemberShipService_IsUserRegistered()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User
                {
                    DeviceRegistrations = new Collection<Device>
                        {
                            new Device()
                        }
                });
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsUserRegistered("test");

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_IsUserRegisteredNoUserName()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User
            {
                DeviceRegistrations = new Collection<Device>
                        {
                            new Device()
                        }
            });
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsUserRegistered("");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Never);
        }

        [TestMethod]
        public void MemberShipService_IsUserRegisteredNoUserFound()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test")));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsUserRegistered("test");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUserNoUserName()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUserNoDeviceResponse()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUserNoUserFound()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "test")));

            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUserNoDeviceFound()
        {
            _userRepository.Setup(s => s.FindUser(It.Is<string>(p => p == "test"))).Returns(new User { DeviceRegistrations = new Collection<Device>() });

            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUserNoAuthenticationRequestFound()
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
                                        Counter = (int) _deviceRegistration.Counter
                                    }
                            }
            });

            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_AuthenticateUser()
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
                                        Counter = (int) _deviceRegistration.Counter
                                    }
                            },
                        AuthenticationRequest = new[]
                        {
                            new AuthenticationRequest
                            {
                                AppId = _startedAuthentication.AppId,
                                KeyHandle = _startedAuthentication.KeyHandle,
                                Challenge = _startedAuthentication.Challenge
                            }
                        }
                    });
            _userRepository.Setup(e => e.RemoveUsersAuthenticationRequests(It.Is<string>(p => p == "test")));
            _userRepository.Setup(e => e.UpdateDeviceCounter(It.Is<string>(p => p == "test"), It.IsAny<byte[]>(), It.IsAny<uint>()));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser("test"), Times.Once);
            _userRepository.Verify(e => e.RemoveUsersAuthenticationRequests(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_IsValidUserNameAndPasswordNullPassword()
        {
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsValidUserNameAndPassword("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemberShipService_IsValidUserNameAndPassword()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User { Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=" });
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsTrue(result);
            _userRepository.Verify(e => e.FindUser(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_IsValidUserNameAndPasswordNoUser()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test")));
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

            Assert.IsFalse(result);
            _userRepository.Verify(e => e.FindUser(It.Is<string>(p => p == "test")), Times.Once);
        }

        [TestMethod]
        public void MemberShipService_IsValidUserNameAndPasswordBadPassword()
        {
            _userRepository.Setup(e => e.FindUser(It.Is<string>(p => p == "test"))).Returns(new User{Password = "notSame"});
            MemberShipService memberShipService = new MemberShipService(_userRepository.Object);

            var result = memberShipService.IsValidUserNameAndPassword("test", "hashedPassword");

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
