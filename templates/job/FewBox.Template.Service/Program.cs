using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FewBox.Template.Service
{
    public class Program
    {
        private static IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogInformation("Wellcome to use FewBox Job!");

            // Get Service and call method
            var job = serviceProvider.GetService<IJob>();
            job.Execute();
        }
    }
}