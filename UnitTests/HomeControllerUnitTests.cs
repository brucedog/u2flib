using System;
using System.Web.Mvc;
using BaseLibrary;
using DataModels;
using DemoU2FSite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class HomeControllerUnitTests
    {
        private Mock<IMemeberShipService> _memeberShipService;

        [TestInitialize]
        public void Setup()
        {
            _memeberShipService = new Mock<IMemeberShipService>();
        }

        [TestMethod]
        public void HomeController_ConstructsProperly()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);

            Assert.IsNotNull(homeController);
        }

        [TestMethod]
        public void HomeController_BeginLoginNoUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel();

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;
            
            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
        }
        
        [TestMethod]
        public void HomeController_BeginLoginNoPassword()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel
                                              {
                                                  UserName = "tester"
                                              };

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
            Assert.AreEqual("tester", ((BeginLoginModel)result.Model).UserName);
        }

        [TestMethod]
        public void HomeController_BeginLoginWithUsernameAndPassword()
        {
            _memeberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            _memeberShipService.Setup(s => s.GenerateServerChallenge(It.Is<string>(p => p == "tester"))).Returns(new ServerChallenge
                                                                                                            {
                                                                                                                AppId = "unittests",
                                                                                                                Challenge = "notrealchallenge",
                                                                                                                Version = "U2F_V2",
                                                                                                                KeyHandle = "notreallykeyhandle"
                                                                                                            });

            HomeController homeController = new HomeController(_memeberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel
            {
                UserName = "tester",
                Password = "password"
            };

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
            
        }

        [TestMethod]
        public void HomeController_Register()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);

            ViewResult result = homeController.Register() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginLoginExceptionThrown()
        {
            _memeberShipService.Setup(s => s.GenerateServerChallenge(It.IsAny<string>())).Throws(new Exception());
            _memeberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(true);

            HomeController homeController = new HomeController(_memeberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel { UserName = "UserName", Password = "Password"};

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompletedLoginExceptionThrown()
        {
            _memeberShipService.Setup(s => s.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            _memeberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(true);

            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel { UserName = "UserName" };

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompletedLoginNoUsername()
        {
            _memeberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(false);

            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel{UserName = string.Empty};

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
            
        }

        [TestMethod]
        public void HomeController_CompletedLoginWithUsername()
        {
            _memeberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            _memeberShipService.Setup(s => s.AuthenticateUser(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "notrealdeviceresponse"))).Returns(true);

            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel
                                                 {
                                                     UserName = "tester",
                                                     DeviceResponse = "notrealdeviceresponse"
                                                 };

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("CompletedLogin", result.ViewName);
            
        }

        [TestMethod]
        public void HomeController_BeginRegisterNoPasswordOrUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            RegisterModel registerModel = new RegisterModel();

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginRegisterBadMatchPasswordsAndUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            RegisterModel registerModel = new RegisterModel
                                          {
                                              UserName = "tester",
                                              Password = "password",
                                              ConfirmPassword = "fail"
                                          };

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginRegisterPasswordsAndUsername()
        {
            _memeberShipService.Setup(
                e => e.GenerateServerRegistration(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "password")))
                .Returns(new ServerRegisterResponse());
            HomeController homeController = new HomeController(_memeberShipService.Object);
            RegisterModel registerModel = new RegisterModel
            {
                UserName = "tester",
                Password = "password",
                ConfirmPassword = "password"
            };

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
            
        }

        [TestMethod]
        public void HomeController_CompleteRegisterNoDeviceTokenOrUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel();

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterExceptionThrown()
        {
            _memeberShipService.Setup(s => s.CompleteRegistration(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel
                                                  {
                                                      UserName = "username",
                                                      DeviceResponse = "DeviceResponse"
                                                  };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithDeviceTokenNoUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel{DeviceResponse = "notrealdevicetoken"};

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameNoDeviceToken()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel { UserName = "tester" };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameAndDeviceToken()
        {
            _memeberShipService.Setup(
                e => e.CompleteRegistration(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "notreallydevicetoken"))).Returns(true);
            HomeController homeController = new HomeController(_memeberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel
                                                  {
                                                      UserName = "tester",
                                                      DeviceResponse = "notreallydevicetoken"
                                                  };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("CompletedRegister", result.ViewName);
            
        }

        [TestMethod]
        public void HomeController_Index()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            ViewResult result = homeController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void HomeController_Login()
        {
            HomeController homeController = new HomeController(_memeberShipService.Object);
            ViewResult result = homeController.Login() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
        }
    }
}