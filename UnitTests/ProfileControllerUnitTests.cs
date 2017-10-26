using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using BaseLibrary;
using DataModels;
using DemoU2FSite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using u2flib.Data;
using u2flib.Data.Messages;
using u2flib.Util;

namespace UnitTests
{
    [TestClass]
    public class ProfileControllerUnitTests
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IMemberShipService> _memberShipService;
        private DeviceRegistration _deviceRegistration;
        private AuthenticateResponse _authenticateResponse;
        private User _user;

        [TestInitialize]
        public void Setup()
        {
            CreateResponses();
            _userRepository = new Mock<IUserRepository>();
            _memberShipService = new Mock<IMemberShipService>();
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
        public void ProfileController_ConstructsProperly()
        {
            ProfileController profileController = new ProfileController(_userRepository.Object, _memberShipService.Object);

            Assert.IsNotNull(profileController);
        }

        private void CreateResponses()
        {
            _deviceRegistration = new DeviceRegistration(
                TestConts.KEY_HANDLE_BASE64_BYTE,
                TestConts.USER_PUBLIC_KEY_AUTHENTICATE_HEX,
                Utils.Base64StringToByteArray(TestConts.ATTESTATION_CERTIFICATE),
                0);

            _authenticateResponse = new AuthenticateResponse(
                TestConts.CLIENT_DATA_AUTHENTICATE_BASE64,
                TestConts.SIGN_RESPONSE_DATA_BASE64,
                TestConts.KEY_HANDLE_BASE64);
        }
    }
}
