using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using FewBox.Template.Service.Model.Services;
using FewBox.Core.Web.Token;
using FewBox.Core.Web.Extension;
using FewBox.Core.Utility.Net;
using System;
using FewBox.Template.Service.Model.Configs;
using FewBox.Template.Service.Model.Repositories;
using FewBox.Template.Service.Domain;
using FewBox.Template.Service.Repository;
using FewBox.Template.Service.Hubs;

namespace FewBox.Template.Service
{
    public partial class Startup
    {
        private IList<ApiVersionDocument> ApiVersionDocuments = new List<ApiVersionDocument> {
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(1, 0),
                    IsDefault = true
                },
                new ApiVersionDocument{
                    ApiVersion = new ApiVersion(2, 0, "alpha1"),
                    IsDefault = false
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
            services.AddFewBox(this.ApiVersionDocuments);
            NetworkUtility.IsCertificateNeedValidate = false;
            NetworkUtility.IsEnsureSuccessStatusCode = false;
            NetworkUtility.IsLogging = true;
            // Biz
            this.ConfigureLowcodeServices(services);
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
