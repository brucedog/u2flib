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
        }
    }
}