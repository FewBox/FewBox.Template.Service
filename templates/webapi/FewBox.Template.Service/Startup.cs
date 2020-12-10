using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FewBox.Core.Web.Extension;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using FewBox.Service.Auth.Model.Configs;
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
            services.AddFewBox(FewBoxDBType.SQLite, FewBoxAuthType.Payload, new ApiVersion(1, 0, "alpha1")); // Todo: Need to change to MySQL.
            // Used for Swagger Open Api Document.
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v1", new[] { "1-alpha", "1-beta", "1" }, "v1");
            });
            services.AddOpenApiDocument(config =>
            {
                this.InitAspNetCoreOpenApiDocumentGeneratorSettings(config, "v2", new[] { "2-alpha", "2-beta", "2" }, "v2");
            });
            // Biz
            services.AddScoped<IAppRepository, AppRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseFewBox(new List<string>{"/swagger/v1/swagger.json","/swagger/v2/swagger.json"} );
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("notificationHub");
            });
        }

        private void InitAspNetCoreOpenApiDocumentGeneratorSettings(AspNetCoreOpenApiDocumentGeneratorSettings config, string documentName, string[] apiGroupNames, string documentVersion)
        {
            config.DocumentName = documentName;
            config.ApiGroupNames = apiGroupNames;
            config.PostProcess = document =>
            {
                this.InitDocumentInfo(document, documentVersion);
            };
            config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));
            config.DocumentProcessors.Add(
                new SecurityDefinitionAppender("JWT", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Bearer [Token]",
                    In = OpenApiSecurityApiKeyLocation.Header
                })
            );
        }

        private void InitDocumentInfo(OpenApiDocument document, string version)
        {
            document.Info.Version = version;
            document.Info.Title = "FewBox Demo Api";
            document.Info.Description = "FewBox shipping, for more information please visit the 'https://fewbox.com'";
            document.Info.TermsOfService = "https://fewbox.com/terms";
            document.Info.Contact = new OpenApiContact
            {
                Name = "FewBox",
                Email = "support@fewbox.com",
                Url = "https://fewbox.com/support"
            };
            document.Info.License = new OpenApiLicense
            {
                Name = "Use under license",
                Url = "https://fewbox.com/license"
            };
            string service = Assembly.GetEntryAssembly().GetName().Name;
            document.Info.ExtensionData = new Dictionary<string, object> {
                {"Service", service}
            };
        }
    }
}
