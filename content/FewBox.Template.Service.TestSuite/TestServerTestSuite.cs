using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FewBox.Service.Shipping.TestSuite
{
    [TestClass]
    public class TestServerTestSuite
    {
        private IWebHostBuilder WebHostBuilder { get; set; }

        [TestInitialize]
        public void Init()
        {
            this.WebHostBuilder =  new WebHostBuilder()
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json");
                    })
                    .UseEnvironment("Test")
                    .UseStartup<Startup>();
        }

        [TestMethod]
        public void TestAPP()
        {
            TestServerWapper(async (client)=>{
                string result = await client.GetStringAsync("/api/apps");
                    Assert.AreEqual(@"{""payload"":[],""isSuccessful"":true,""errorMessage"":null,""errorCode"":null}", result);
            });
        }

        private void TestServerWapper(Action<HttpClient> action)
        {
            using (var server = new TestServer(this.WebHostBuilder))
            {
                using (var client = server.CreateClient())
                {
                    action(client);
                }
            }
        }
    }
}