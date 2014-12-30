using System;
using System.Web.Mvc;
using DemoU2FSite.Controllers;
using DemoU2FSite.Models;
using DemoU2FSite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace UnitTests
{
    [TestClass]
    public class HomeControllerUnitTests
    {
        private IMemeberShipService _memeberShipService;

        [TestInitialize]
        public void Setup()
        {
            _memeberShipService = MockRepository.GenerateMock<IMemeberShipService>();
        }

        [TestMethod]
        public void HomeController_ConstructsProperly()
        {
            HomeController homeController = new HomeController(_memeberShipService);

            Assert.IsNotNull(homeController);
        }

        [TestMethod]
        public void HomeController_BeginLoginNoUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService);
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
            HomeController homeController = new HomeController(_memeberShipService);
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
            _memeberShipService.Stub(s => s.IsUserRegistered(Arg<string>.Is.Equal("tester"), Arg<string>.Is.Equal("password"))).Return(true);
            _memeberShipService.Expect(s => s.GenerateServerChallenge(Arg<string>.Is.Equal("tester"))).Return(new ServerChallenge
                                                                                                            {
                                                                                                                AppId = "unittests",
                                                                                                                Challenge = "notrealchallenge",
                                                                                                                Version = "U2F_V2",
                                                                                                                KeyHandle = "notreallykeyhandle"
                                                                                                            });

            HomeController homeController = new HomeController(_memeberShipService);
            BeginLoginModel beginLoginModel = new BeginLoginModel
            {
                UserName = "tester",
                Password = "password"
            };

            ViewResult result = homeController.BeginLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
            _memeberShipService.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeController_CompletedLoginExceptionThrown()
        {
            _memeberShipService.Expect(s => s.AuthenticateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Throw(new Exception());

            HomeController homeController = new HomeController(_memeberShipService);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel { UserName = string.Empty };

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompletedLoginNoUsername()
        {
            _memeberShipService.Expect(s => s.IsUserRegistered(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(false);

            HomeController homeController = new HomeController(_memeberShipService);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel{UserName = string.Empty};

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishLogin", result.ViewName);
            _memeberShipService.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeController_CompletedLoginWithUsername()
        {
            _memeberShipService.Stub(s => s.IsUserRegistered(Arg<string>.Is.Equal("tester"), Arg<string>.Is.Anything)).Return(true);
            _memeberShipService.Expect(s => s.AuthenticateUser(Arg<string>.Is.Equal("tester"), Arg<string>.Is.Equal("notrealdeviceresponse"))).Return(true);

            HomeController homeController = new HomeController(_memeberShipService);
            CompleteLoginModel beginLoginModel = new CompleteLoginModel
                                                 {
                                                     UserName = "tester",
                                                     DeviceResponse = "notrealdeviceresponse"
                                                 };

            ViewResult result = homeController.CompletedLogin(beginLoginModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("CompletedLogin", result.ViewName);
            _memeberShipService.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeController_BeginRegisterNoPasswordOrUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            RegisterModel registerModel = new RegisterModel();

            ViewResult result = homeController.BeginRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("Register", result.ViewName);
        }

        [TestMethod]
        public void HomeController_BeginRegisterBadMatchPasswordsAndUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService);
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
            _memeberShipService.Expect(
                e => e.GenerateServerRegisteration(Arg<string>.Is.Equal("tester"), Arg<string>.Is.Equal("password")))
                .Return(new ServerRegisterResponse());
            HomeController homeController = new HomeController(_memeberShipService);
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
            _memeberShipService.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeController_CompleteRegisterNoDeviceTokenOrUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            CompleteRegisterModel registerModel = new CompleteRegisterModel();

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterExceptionThrown()
        {
            _memeberShipService.Expect(s => s.AuthenticateUser(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Throw(new Exception());
            HomeController homeController = new HomeController(_memeberShipService);
            CompleteRegisterModel registerModel = new CompleteRegisterModel();

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithDeviceTokenNoUsername()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            CompleteRegisterModel registerModel = new CompleteRegisterModel{DeviceResponse = "notrealdevicetoken"};

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameNoDeviceToken()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            CompleteRegisterModel registerModel = new CompleteRegisterModel { UserName = "tester" };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsFalse(homeController.ModelState.IsValid);
            Assert.AreEqual("FinishRegister", result.ViewName);
        }

        [TestMethod]
        public void HomeController_CompleteRegisterWithUsernameAndDeviceToken()
        {
            _memeberShipService.Expect(
                e => e.CompleteRegisteration(Arg<string>.Is.Equal("tester"), Arg<string>.Is.Equal("notreallydevicetoken")));
            HomeController homeController = new HomeController(_memeberShipService);
            CompleteRegisterModel registerModel = new CompleteRegisterModel
                                                  {
                                                      UserName = "tester",
                                                      DeviceResponse = "notreallydevicetoken"
                                                  };

            ViewResult result = homeController.CompleteRegister(registerModel) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(homeController.ModelState.IsValid);
            Assert.AreEqual("CompletedRegister", result.ViewName);
            _memeberShipService.VerifyAllExpectations();
        }

        [TestMethod]
        public void HomeController_Index()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            ViewResult result = homeController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void HomeController_Login()
        {
            HomeController homeController = new HomeController(_memeberShipService);
            ViewResult result = homeController.Login() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ViewName);
        }
    }
}