    
using FewBox.Template.Service.Controllers;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FewBox.Template.Service.UnitTest
{
    [TestClass]
    public class HealthyControllerUnitTest
    {
        private HealthyController HealthyController { get; set; }

        [TestInitialize]
        public void Init()
        {
            // E.G: l.Method(It.IsAny<string>());
            var appService = Mock.Of<IAppService>(l=>
                l.GetHealtyInfo()==new HealthyDto{ Version = "1.0.1" });
            this.HealthyController = new HealthyController(appService);
        }


        [TestMethod]
        public void TestGet()
        {
            var response = this.HealthyController.Get();
            Assert.AreEqual("1.0.1", response.Payload.Version);
        }
    }
}