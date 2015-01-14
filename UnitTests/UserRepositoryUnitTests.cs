using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using DemoU2FSite.Repository;
using DemoU2FSite.Repository.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using u2flib.Data.Messages;
using DeviceRegistration = u2flib.Data.DeviceRegistration;

namespace UnitTests
{
    [TestClass]
    public class UserRepositoryUnitTests
    {
        private Mock<IDataContext> _mockContext;
        private StartedRegistration _startedRegistration;
        private RegisterResponse _registerResponse;
        private RawRegisterResponse _rawAuthenticateResponse;
        private DeviceRegistration _deviceRegistration;
        private AuthenticateResponse _authenticateResponse;
        private StartedAuthentication _startedAuthentication;

        [TestInitialize]
        public void Setup()
        {
            CreateResponses();
            var userData = new List<User>
                {
                    new User {Name = "TestUserJustName"},
                    new User
                        {
                            Name = "UserComplete",
                            Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=",
                            DeviceRegistrations = new Collection<DemoU2FSite.Repository.DeviceRegistration>
                                {
                                    new DemoU2FSite.Repository.DeviceRegistration
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
                        },
                    new User {Name = "AAA"},
                }.AsQueryable();

            var userMockDbSet = new Mock<DbSet<User>>();
            userMockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            userMockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            userMockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            userMockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            _mockContext = new Mock<IDataContext>();
            _mockContext.Setup(c => c.Users).Returns(userMockDbSet.Object);
        }

        [TestMethod]
        public void UserRepository_ConstructsProperly()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            Assert.IsNotNull(userRepository);
        }

        [TestMethod]
        public void UserRepository_FindUserNullReturnsNull()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            var user = userRepository.FindUser(null);

            Assert.IsNull(user);
        }

        [TestMethod]
        public void UserRepository_FindUserEmptyReturnsNull()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            var user = userRepository.FindUser("");

            Assert.IsNull(user);
        }

        [TestMethod]
        public void UserRepository_FindUser()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            var user = userRepository.FindUser("TestUserJustName");

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void UserRepository_AddUser()
        {
            _mockContext.Setup(s => s.Users.Add(It.IsAny<User>()));
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            userRepository.AddUser("NewUser", "NewPassword");

            _mockContext.Verify(v => v.Users.Add(It.Is<User>((t => t.Name == "NewUser" && t.Password == "NewPassword"))), Times.Once);
            _mockContext.Verify(v => v.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void UserRepository_UpdateDeviceCounterNoUserFound()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            userRepository.UpdateDeviceCounter("NoUser", new byte[0], 1);

            // Nothing saved because user not found
            _mockContext.Verify(v => v.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void UserRepository_UpdateDeviceCounter()
        {
            UserRepository userRepository = new UserRepository(_mockContext.Object);

            userRepository.UpdateDeviceCounter("UserComplete", _deviceRegistration.PublicKey, 1);

            _mockContext.Verify(v => v.SaveChanges(), Times.Once);
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