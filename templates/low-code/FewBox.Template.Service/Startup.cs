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
using FewBox.Template.Service.Middlewares;
using FewBox.Template.Service.Model.Configs;
using FewBox.Template.Service.Domain;

namespace FewBox.Template.Service
{
    public partial class Startup
    {
        private IList<ApiVersionDocument> ApiVersionDocuments = new List<ApiVersionDocument> {
                new ApiVersionDocument{
                    ApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0),
                    IsDefault = true
                },
                new ApiVersionDocument{
                    ApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(2, 0, "alpha1"),
                    IsDefault = false
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
            services.AddFewBox(this.ApiVersionDocuments, FewBoxDBType.SQLite, FewBoxAuthType.Payload);
            RestfulUtility.IsCertificateNeedValidate = false;
            RestfulUtility.IsEnsureSuccessStatusCode = false;
            RestfulUtility.IsLogging = true;
            HttpUtility.IsCertificateNeedValidate = false;
            HttpUtility.IsEnsureSuccessStatusCode = false;
            // Biz
            services.AddScoped<IAppRepository, AppRepository>();
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
