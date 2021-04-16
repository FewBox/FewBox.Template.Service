using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FewBox.Core.Web.Extension;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;
using FewBox.Template.Service.Model.Repositories;
using FewBox.Template.Service.Model.Services;
using FewBox.Template.Service.Repository;
using FewBox.Template.Service.Hubs;
using FewBox.SDK.Extension;
using FewBox.SDK.Auth;

namespace FewBox.Template.Service
{
    public class Startup
    {
        private IList<ApiVersionDocument> ApiVersionDocuments = new List<ApiVersionDocument> {
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(1, 0),
                    IsDefault = true
                },
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(2, 0, "alpha1")
                },
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(2, 0, "beta1")
                }
            };
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFewBox(this.ApiVersionDocuments, FewBoxDBType.SQLite, FewBoxAuthType.Payload); // Todo: Need to change to MySQL.
            // Biz
            services.AddScoped<IAppRepository, AppRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFewBox(this.ApiVersionDocuments);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("notificationHub");
            });
        }
    }
}
