using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using FewBox.Template.Service.Engine;

namespace FewBox.Template.Service.Boot.FewBox
{
    [Generator]
    class FewBoxBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            this.GenerateStartup(context);
            this.GenerateController(context);
            this.GenerateMapper(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            System.Diagnostics.Debugger.Launch();
        }

        private void GenerateStartup(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
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
            using FewBox.Template.Service.Domain;
            using FewBox.Template.Service.Model.Configs;
            using FewBox.SDK.Extension;
            using FewBox.SDK.Auth;

            namespace FewBox.Template.Service
            {{
                public class Startup
                {{
                    private IList<ApiVersionDocument> ApiVersionDocuments = new List<ApiVersionDocument> {{
                            new ApiVersionDocument{{
                                ApiVersion = new ApiVersion(1, 0),
                                IsDefault = true
                            }},
                            new ApiVersionDocument{{
                                ApiVersion = new ApiVersion(2, 0, ""alpha1"")
                            }},
                            new ApiVersionDocument{{
                                ApiVersion = new ApiVersion(2, 0, ""beta1"")
                            }}
                        }};
                    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
                    {{
                        this.Configuration = configuration;
                        this.Environment = environment;
                    }}

                    public IConfiguration Configuration {{ get; }}
                    public IWebHostEnvironment Environment {{ get; }}

                    // This method gets called by the runtime. Use this method to add services to the container.
                    public void ConfigureServices(IServiceCollection services)
                    {{
                        services.AddFewBox(this.ApiVersionDocuments, FewBoxDBType.SQLite, FewBoxAuthType.Payload); // Todo: Need to change to MySQL.
                        // Config
                        var lowCodeConfig = this.Configuration.GetSection(""LowCode"").Get<LowCodeConfig>();
                        services.AddSingleton(lowCodeConfig);
                        // Biz
            ");
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                sourceBuilder.Append($@"services.AddScoped<I{entityName}Repository, {entityName}Repository>();");
            }
            var serviceFiles = LowCodeEngine.GetServiceFiles(context);
            foreach (var serviceFile in serviceFiles)
            {
                string serviceName = LowCodeEngine.GetFileName(serviceFile);
                sourceBuilder.Append($@"services.AddScoped<I{serviceName}Service, {serviceName}Service>();");
            }
            sourceBuilder.Append($@"
                        }}

                    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
                    {{
                        app.UseFewBox(this.ApiVersionDocuments);
                        app.UseEndpoints(endpoints =>
                        {{
                            endpoints.MapHub<NotificationHub>(""notificationHub"");
                        }});
                    }}
                }}
            }}
            ");

            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            string hintName = $"Startup.cs";
            context.AddSource(hintName, sourceText);
        }

        private void GenerateController(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using AutoMapper;
                using FewBox.Core.Web.Controller;
                using FewBox.Template.Service.Model.Dtos;
                using FewBox.Template.Service.Model.Entities;
                using FewBox.Template.Service.Model.Repositories;
                using Microsoft.AspNetCore.Authorization;
                using Microsoft.AspNetCore.Mvc;

                namespace FewBox.Template.Service.Controllers
                {{
                    [Route(""api/v{{v:apiVersion}}/[controller]"")]
                    [Authorize(Policy=""JWTRole_ControllerAction"")]
                    public class {entityName}sController : ResourcesController<I{entityName}Repository, {entityName}, {entityName}Dto, {entityName}PersistantDto>
                    {{
                        public {entityName}sController(I{entityName}Repository {entityName.ToLower()}Repository, IMapper mapper) : base({entityName.ToLower()}Repository, mapper)
                        {{
                            // SQLite ID must be Upcase.
                        }}
                    }}
                }}");
                var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
                string hintName = $"{entityName}sController.cs";
                context.AddSource(hintName, sourceText);
            }
        }

        private void GenerateMapper(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
                using AutoMapper;
                using FewBox.Template.Service.Model.Dtos;
                using FewBox.Template.Service.Model.Entities;

                namespace FewBox.Template.Service.AutoMapperProfiles
                {{
                    public class MapperProfiles : Profile
                    {{
                        public MapperProfiles()
                        {{");
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                sourceBuilder.AppendLine($@"
                            CreateMap<{entityName}, {entityName}Dto>();
                            CreateMap<{entityName}PersistantDto, {entityName}>();
                    ");
            }
            sourceBuilder.Append($@"}}
                    }}
                }}");
            context.AddSource($"MapperProfiles.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}