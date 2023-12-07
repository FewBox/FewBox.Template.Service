/* Company: FewBox */
/* Copyright FewBox */
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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
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
