
using System.Threading.Tasks;
using FewBox.Core.Utility.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Service.Shipping.TestSuite
{
    [TestClass]
    public class TestServerTestSuite
    {
        [TestMethod]
        public async Task TestGet()
        {
            var webHostBuilder =
              new WebHostBuilder()
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json");
                    })
                    .UseEnvironment("Test")
                    .UseStartup<Startup>();
            using (var server = new TestServer(webHostBuilder))
            {
                using (var client = server.CreateClient())
                {
                    string result = await client.GetStringAsync("/api/apps");
                    Assert.AreEqual(@"{""payload"":[],""isSuccessful"":true,""errorMessage"":null,""errorCode"":null}", result);
                }
            }
        }
    }
}