using DemoU2FSite.Repository;
using DemoU2FSite.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace UnitTests
{
    [TestClass]
    public class MemeberShipServiceUnitTests
    {
        private IDataContext _dataContext;

        [TestInitialize]
        public void SetUp()
        {
            _dataContext = MockRepository.GenerateMock<IDataContext>();
        }

        [TestMethod]
        public void MemeberShipService_ConstructsProperly()
        {
            MemeberShipService memeberShipService = new MemeberShipService(_dataContext);

            Assert.IsNotNull(memeberShipService);
        }
    }
}
