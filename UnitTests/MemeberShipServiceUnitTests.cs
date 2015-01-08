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

        [TestMethod]
        public void MemeberShipService_AuthenticateUser()
        {
            _userRepository.Expect(e => e.FindUser(Arg<string>.Is.Equal("test"))).Return(new User { Password = "KSpjLUfp4gaP1Zu4F+6qhcBNhQeJJLRnN1zt9MBHWh8=" });
            _userRepository.Expect(e => e.UpdateDeviceCounter(Arg<string>.Is.Equal("test"), Arg<byte[]>.Is.Anything, Arg<uint>.Is.Anything));
            MemeberShipService memeberShipService = new MemeberShipService(_userRepository);

            var result = memeberShipService.AuthenticateUser("test", "deviceResponseWithPublicKey");

            Assert.IsTrue(result);
            _userRepository.VerifyAllExpectations();
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
