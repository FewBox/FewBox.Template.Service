/* Company: FB_BRAND */
/* FB_COPYRIGHT */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace FewBox.Template.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                /*.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })*/
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(options =>
                    {
                        options.BeforeSend = @event =>
                        {
                            return @event;
                        };
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
