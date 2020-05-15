using FewBox.Core.Utility.Net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace FewBox.Template.Service
{
    public class Program
    {
        private static IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
            Configuration = builder.Build();
            var endpoint = Configuration.GetSection("Endpoint").Get<Endpoint>();
            string url = $"{endpoint.Protocol}://{endpoint.Host}";
            Console.WriteLine(url);
            string response = HttpUtility.Get(url, new List<Header> { });
            Console.WriteLine("Hello FewBox!");
        }

        public class Endpoint
        {
            public string Protocol { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }
    }
}