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
                string port = this.Job.Endpoint.Port != 0 ? $":{this.Job.Endpoint.Port}" : String.Empty;
                string url = $"{this.Job.Endpoint.Protocol}://{this.Job.Endpoint.Host}{port}";
                Console.WriteLine(url);
                string response = HttpUtility.Get(url, new List<Header> { });
                Console.WriteLine("Hello FewBox!");
            }
            catch (Exception exception)
            {
                this.Logger.LogError(exception.Message);
            }
        }
    }
}