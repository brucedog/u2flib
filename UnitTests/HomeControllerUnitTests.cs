using System;
using System.Collections.Generic;
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
        private Mock<IMemberShipService> _memberShipService;

        [TestInitialize]
        public void Setup()
        {
            _memberShipService = new Mock<IMemberShipService>();
        }

        [TestMethod]
        public void HomeController_BeginLoginNoUsername()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel();

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;
            
            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginLoginException()
        {
            _memberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            _memberShipService.Setup(s => s.IsValidUserNameAndPassword(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "password"))).Returns(true).Verifiable();
            _memberShipService.Setup(s => s.GenerateServerChallenges(It.Is<string>(p => p == "tester")))
                .Returns(new List<ServerChallenge>());

            HomeController homeController = new HomeController(_memberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel
            {
                UserName = "tester",
                Password = "password"
            };

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
        }
        
        [TestMethod]
        public void HomeController_BeginLoginNoPassword()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
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
            _memberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            _memberShipService.Setup(s => s.GenerateServerChallenges(It.Is<string>(p => p == "tester")))
                .Returns(new List<ServerChallenge>
            {
                new ServerChallenge
                {
                    appId = "unittests",
                    challenge = "notrealchallenge",
                    version = "U2F_V2",
                    keyHandle = "notreallykeyhandle",
                }
            }).Verifiable();
            _memberShipService.Setup(s => s.IsValidUserNameAndPassword(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "password"))).Returns(true).Verifiable();

            HomeController homeController = new HomeController(_memberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel
            {
                UserName = "tester",
                Password = "password"
            };

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
            _memberShipService.VerifyAll();
        }

        [TestMethod]
        public void HomeController_Register()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);

            ViewResult result = homeController.Register() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginLoginExceptionThrown()
        {
            _memberShipService.Setup(s => s.GenerateServerChallenges(It.IsAny<string>())).Throws(new Exception());
            _memberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(true);

            HomeController homeController = new HomeController(_memberShipService.Object);
            BeginLoginModel beginLoginModel = new BeginLoginModel { UserName = "UserName", Password = "Password"};

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Login", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompletedLoginExceptionThrown()
        {
            _memberShipService.Setup(s => s.AuthenticateUser(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            _memberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(true);

            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel { UserName = "UserName" };

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompletedLoginNoUsername()
        {
            _memberShipService.Setup(s => s.IsUserRegistered(It.IsAny<string>())).Returns(false);

            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel{UserName = string.Empty};

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
        }

        // ignore until i can mock the form auth 
        [Ignore]
        [TestMethod]
        public void HomeController_CompletedLoginWithUsername()
        {
            _memberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            _memberShipService.Setup(s => s.AuthenticateUser(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "notrealdeviceresponse"))).Returns(true);

            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel
                                                 {
                                                     UserName = "tester",
                                                     DeviceResponse = "notrealdeviceresponse"
                                                 };

            var result = homeController.CompletedLogin(beginLoginModel) as RedirectToRouteResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"] as string);
        }

        [TestMethod]
        public void HomeController_BeginRegisterNoPasswordOrUsername()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
            RegisterModel registerModel = new RegisterModel();

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginRegisterBadMatchPasswordsAndUsername()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
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
            _memberShipService.Setup(
                e => e.GenerateServerChallenge(It.Is<string>(p => p == "tester")))
                .Returns(new ServerRegisterResponse());
            HomeController homeController = new HomeController(_memberShipService.Object);
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
        public void HomeController_BeginRegisterDuplicateUser()
        {
            _memberShipService.Setup(s => s.IsUserRegistered(It.Is<string>(p => p == "tester"))).Returns(true);
            HomeController homeController = new HomeController(_memberShipService.Object);

            RegisterModel registerModel = new RegisterModel
            {
                UserName = "tester",
                Password = "password",
                ConfirmPassword = "password"
            };

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterNoDeviceTokenOrUsername()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel();

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterExceptionThrown()
        {
            _memberShipService.Setup(s => s.CompleteRegistration(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            HomeController homeController = new HomeController(_memberShipService.Object);
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
            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel{DeviceResponse = "notrealdevicetoken"};

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameNoDeviceToken()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
            CompleteRegisterModel registerModel = new CompleteRegisterModel { UserName = "tester" };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameAndDeviceToken()
        {
            _memberShipService.Setup(
                e => e.CompleteRegistration(It.Is<string>(p => p == "tester"), It.Is<string>(p => p == "notreallydevicetoken"))).Returns(true);
            HomeController homeController = new HomeController(_memberShipService.Object);
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
            HomeController homeController = new HomeController(_memberShipService.Object);
            ViewResult result = homeController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void HomeController_Login()
        {
            HomeController homeController = new HomeController(_memberShipService.Object);
            ViewResult result = homeController.Login() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
        }
    }
}