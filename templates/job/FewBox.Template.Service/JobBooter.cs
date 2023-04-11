using System;
using Microsoft.Extensions.Logging;
using FewBox.Core.Utility.Net;
using System.Collections.Generic;

namespace FewBox.Template.Service
{
    public class JobBooter : IJobBooter
    {
        private JobConfig Job { get; set; }
        private ILogger Logger { get; set; }

        public JobBooter(JobConfig job, ILogger<JobBooter> logger)
        {
            this.Job = job;
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