using System.Collections.ObjectModel;
using System.Linq;
using DemoU2FSite.Repository;
using DemoU2FSite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using u2flib.Data.Messages;
using u2flib.Util;

namespace UnitTests
{
    [TestClass]
    public class MemeberShipServiceUnitTests
    {
        private IUserRepository _userRepository;
        private RegisterResponse _registerResponse;
        private RawRegisterResponse _rawAuthenticateResponse;
        private u2flib.Data.DeviceRegistration _deviceRegistration;
        private AuthenticateResponse _authenticateResponse;
        private User _user;
        private StartedRegistration _startedRegistration;
        private StartedAuthentication _startedAuthentication;

        [TestInitialize]
        public void SetUp()
        {
            CreateResponses();
            _userRepository = MockRepository.GenerateMock<IUserRepository>();
            _user = new User
                {
                    Name = "test",
                    Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                    DeviceRegistrations = new Collection<DeviceRegistration>
                        {
                            new DeviceRegistration
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
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            Assert.IsNotNull(memeberShipService);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeSucess()
        {
            _userRepository.Expect(e => e.FindUser("test")).Return(_user);
            _userRepository.Expect(
                e =>
                e.SaveUserAuthenticationRequest(Arg<string>.Is.Equal("test"), Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                                                Arg<string>.Is.Anything));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNotNull(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoDeviceFound()
        {
            _userRepository.Expect(e => e.FindUser("test")).Return(new User { DeviceRegistrations = new Collection<DeviceRegistration>() });
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNull(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoUserFound()
        {
            _userRepository.Expect(e => e.FindUser("test")).Return(null);
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerChallenge("test");

            Assert.IsNull(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerChallengeNoUsername()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerChallenge("");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistration()
        {
            _userRepository.Expect(e => e.AddUser(Arg<string>.Is.Equal("test"), Arg<string>.Is.Anything));
            _userRepository.Expect(e => e.AddAuthenticationRequest(Arg<string>.Is.Equal("test"), Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerRegistration("test", "password");

            Assert.IsNotNull(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistrationNoPassword()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerRegistration("test", "");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_GenerateServerRegistrationNoUserName()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.GenerateServerRegistration("", "password");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationSucess()
        {
            DeviceRegistration deviceRegistration = _user.DeviceRegistrations.First();
            _user.AuthenticationRequest.Challenge = _startedRegistration.Challenge;
            _user.AuthenticationRequest.AppId = _startedRegistration.AppId;
            
            _userRepository.Expect(e => e.FindUser("test")).Return(_user);
            _userRepository.Expect(e => e.RemoveUsersAuthenticationRequest("test"));
            _userRepository.AddDeviceRegistration("Test", deviceRegistration.AttestationCert, deviceRegistration.Counter, deviceRegistration.KeyHandle, deviceRegistration.PublicKey);
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.CompleteRegistration("test", _registerResponse.ToJson());

            Assert.IsTrue(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoUserFound()
        {
            _userRepository.Expect(e => e.FindUser("test")).Return(null);
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationUserFoundWithNoAuthenticationRequest()
        {
            _userRepository.Expect(e => e.FindUser("test")).Return(new User());
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.CompleteRegistration("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
            _userRepository.VerifyAllExpectations();
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoDeviceResponse()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.CompleteRegistration("test", "");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_CompleteRegistrationNoUserName()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.CompleteRegistration("", "nothing");

            Assert.IsFalse(result);
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

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoUserName()
        {
            CreateResponses();

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoDeviceResponse()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoUserFound()
        {
            _userRepository.Stub(s => s.FindUser(Arg<string>.Is.Equal("test"))).Return(null);

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoDeviceFound()
        {
            _userRepository.Stub(s => s.FindUser(Arg<string>.Is.Equal("test"))).Return(new User{DeviceRegistrations = new Collection<DeviceRegistration>()});

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUserNoAuthenticationRequestFound()
        {
            _userRepository.Stub(s => s.FindUser(Arg<string>.Is.Equal("test"))).Return(new User
            {
                DeviceRegistrations = new Collection<DeviceRegistration>
                            {
                                new DeviceRegistration
                                    {
                                        KeyHandle = _deviceRegistration.KeyHandle,
                                        PublicKey = _deviceRegistration.PublicKey,
                                        AttestationCert = _deviceRegistration.AttestationCert,
                                        Counter = _deviceRegistration.Counter
                                    }
                            }
            });

            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MemeberShipService_AuthenticateUser()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(
                new User
                    {
                        Name = "test",
                        Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                        DeviceRegistrations = new Collection<DeviceRegistration>
                            {
                                new DeviceRegistration
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
                    });
            _userRepository.Expect(e => e.RemoveUsersAuthenticationRequest(Arg<string>.Is.Equal("test")));
            _userRepository.Expect(e => e.UpdateDeviceCounter(Arg<string>.Is.Equal("test"), Arg<byte[]>.Is.Anything, Arg<uint>.Is.Anything));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", _authenticateResponse.ToJson());

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

        private void CreateResponses()
        {
            _startedRegistration = new StartedRegistration(TestConts.SERVER_CHALLENGE_REGISTER_BASE64, TestConts.APP_ID_ENROLL);
            _registerResponse = new RegisterResponse(TestConts.REGISTRATION_RESPONSE_DATA_BASE64,
                                                                     TestConts.CLIENT_DATA_REGISTER_BASE64);
            _rawAuthenticateResponse = RawRegisterResponse.FromBase64(_registerResponse.RegistrationData);
            _deviceRegistration = _rawAuthenticateResponse.CreateDevice();

            _authenticateResponse = new AuthenticateResponse(TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                                                            TestConts.SIGN_RESPONSE_DATA_BASE64,
                                                            TestConts.KEY_HANDLE_BASE64);

            _startedAuthentication = new StartedAuthentication(TestConts.SERVER_CHALLENGE_SIGN_BASE64, TestConts.APP_ID_ENROLL,
                                          TestConts.KEY_HANDLE_BASE64);
        }
    }
}
