using System;
using Microsoft.Extensions.Logging;

namespace FewBox.Template.Service
{
    public class Job : IJob
    {
        private Endpoint Endpoint { get; set; }
        private ILogger Logger { get; set; }

        public Job(Endpoint endpoint, ILogger<Job> logger)
        {
            this.Endpoint = endpoint;
            this.Logger = logger;
        }

        public void Execute()
        {
            try
            {
                string url = $"{this.Endpoint.Protocol}://{this.Endpoint.Host}:{this.Endpoint.Port}";
                Console.WriteLine(url);
                string response = HttpUtility.Get(url, new List<Header> { });
                Console.WriteLine("Hello FewBox!");
            }
            catch(Exception exception)
            {
                this.Logger.LogError(exception.Message);
            }
        }
    }
}