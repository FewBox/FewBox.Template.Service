using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FewBox.Template.Service
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var endpoint = Configuration.GetSection("Endpoint").Get<Endpoint>();
            services.AddSingleton(endpoint);
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddConfiguration(this.Configuration.GetSection("Logging"));
            })
            .Configure<LoggerFilterOptions>(this.Configuration);
            services.AddSingleton<IJob, Job>();
        }
    }
}