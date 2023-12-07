using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FewBox.Service.Boot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FewBox.Template.Service.Boot
{
    [Generator]
    class FewBoxBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            LowCodeEngine.GenerateDebug(context);
            this.GenerateStartup(context);
            this.GenerateControllers(context);
            this.GenerateMapper(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            Debug.WriteLine("Initalize code generator");
        }

        private void GenerateStartup(GeneratorExecutionContext context)
        {
            string @namespace = LowCodeEngine.GetNamespace(context);
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
            using Microsoft.Extensions.DependencyInjection;
            using {@namespace}.Model.Repositories;
            using {@namespace}.Model.Services;
            using {@namespace}.Repository;
            using {@namespace}.Domain;

            namespace {@namespace}
            {{
                public partial class Startup
                {{
                    // This method gets called by the runtime. Use this method to add services to the container.
                    private void ConfigureLowcodeServices(IServiceCollection services)
                    {{
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
                }}
            }}
            ");
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            string hintName = $"Startup.g.cs";
            context.AddSource(hintName, sourceText);
        }

        private void GenerateControllers(GeneratorExecutionContext context)
        {
            string @namespace = LowCodeEngine.GetNamespace(context);
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                string controllerName;
                if (entityName.EndsWith("y"))
                {
                    controllerName = $"{entityName.Substring(0, entityName.Length - 1)}ies";
                }
                else if (entityName.EndsWith("sh") || entityName.EndsWith("ch"))
                {
                    controllerName = $"{entityName}es";
                }
                else
                {
                    controllerName = $"{entityName}s";
                }
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using AutoMapper;
                using FewBox.Core.Web.Controller;
                using FewBox.Core.Web.Token;
                using {@namespace}.Model.Dtos;
                using {@namespace}.Model.Entities;
                using {@namespace}.Model.Repositories;
                using Microsoft.AspNetCore.Authorization;
                using Microsoft.AspNetCore.Mvc;

                namespace {@namespace}.Controllers
                {{
                    [Route(""api/v{{v:apiVersion}}/[controller]"")]
                    [Authorize(Policy=""JWTPayload_ControllerAction"")]
                    public partial class {controllerName}Controller : ResourcesController<I{entityName}Repository, {entityName}, {entityName}Dto, {entityName}PersistantDto>
                    {{
                        public {controllerName}Controller(I{entityName}Repository {entityName.ToLower()}Repository, ITokenService tokenService, IMapper mapper) : base({entityName.ToLower()}Repository, tokenService, mapper)
                        {{
                            // SQLite ID must be Upcase.
                        }}
                    }}
                }}");
                var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);

                string hintName = $"{controllerName}Controller.g.cs";
                context.AddSource(hintName, sourceText);
            }
            var schemaScriptBuilder = new StringBuilder();
            foreach (var entityFile in entityFiles)
            {
                var fieldBuilder = new StringBuilder();
                string entityName = LowCodeEngine.GetFileName(entityFile);
                string tableName = entityName.ToLower();
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                foreach (string key in entityMetaManifest.Keys)
                {
                    var entity = entityMetaManifest[key];
                    fieldBuilder.Append($" `{key}` {entity.DbType}({entity.Length}) DEFAULT NULL,\r\n");
                }
                schemaScriptBuilder.Append($@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                        `Id` char(36) NOT NULL,
                        {fieldBuilder.ToString().Trim()}
                        `CreatedBy` char(36) DEFAULT NULL,
                        `ModifiedBy` char(36) DEFAULT NULL,
                        `CreatedTime` datetime DEFAULT NULL,
                        `ModifiedTime` datetime DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                    );
                ");
            }
            var lowCodeController = $@"
                using FewBox.Core.Web.Dto;
                using Microsoft.AspNetCore.Mvc;

                namespace {@namespace}.Controllers
                {{
                    public partial class LowCodeController : ControllerBase
                    {{
                        [HttpGet(""schema"")]
                        public PayloadResponseDto<string> Schema()
                        {{
                            return new PayloadResponseDto<string>{{
                                IsSuccessful = true,
                                Payload = $@""{schemaScriptBuilder}""
                            }};
                        }}
                    }}
                }}";
            var lowCodeSourceText = SourceText.From(lowCodeController, Encoding.UTF8);
            context.AddSource("LowCodeController.g.cs", lowCodeSourceText);
        }

        private void GenerateMapper(GeneratorExecutionContext context)
        {
            string @namespace = LowCodeEngine.GetNamespace(context);
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
                using AutoMapper;
                using {@namespace}.Model.Dtos;
                using {@namespace}.Model.Entities;

                namespace {@namespace}.AutoMapperProfiles
                {{
                    public partial class RepositoryMapperProfiles : Profile
                    {{
                        public RepositoryMapperProfiles()
                        {{");
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                sourceBuilder.AppendLine($@"
                            CreateMap<{entityName}, {entityName}Dto>();
                            CreateMap<{entityName}PersistantDto, {entityName}>();
                    ");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    if (type.EndsWith("Type", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sourceBuilder.AppendLine($@"
                            CreateMap<{key}, {key}Dto>();
                            CreateMap<{key}Dto, {key}>();
                    ");
                    }
                }
            }
            sourceBuilder.Append($@"}}
                    }}
                }}");
            context.AddSource($"RepositoryMapperProfiles.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

    }
}