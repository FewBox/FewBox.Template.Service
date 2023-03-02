using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FewBox.Service.Shipping.Middlewares
{
    public class FewBoxMiddleware
    {
        private RequestDelegate RequestDelegate { get; set; }
        private ILogger Logger { get; set; }
        public FewBoxMiddleware(RequestDelegate requestDelegate, ILogger<FewBoxMiddleware> logger)
        {
            this.RequestDelegate = requestDelegate;
            this.Logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine(context.Request.Method);
            await this.RequestDelegate.Invoke(context);
        }
    }
}