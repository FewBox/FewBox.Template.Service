using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using FewBox.Template.Service.Engine;

namespace FewBox.Template.Service.Boot
{
    [Generator]
    class FewBoxBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            this.GenerateJsonFiles(context);
            this.GenerateDtos(context);
            this.GenerateEntities(context);
            this.GenerateRepositories(context);
            this.GenerateStartup(context);
            this.GenerateControllers(context);
            this.GenerateMapper(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            Debug.WriteLine("Initalize code generator");
        }

        private void GenerateJsonFiles(GeneratorExecutionContext context)
        {
            string jsonFiles = String.Join(",", LowcodeEngine.GetEntityFiles(context).Select(file => Path.GetFileNameWithoutExtension(file.Path)));
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
            namespace {Consts.Namespace}.Boot
            {{
                public static class Lowcode
                {{
                    public static string GetJsonFiles()
                    {{
                        return @""{jsonFiles}"";
                    }}
                }}
            }}");
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            string hintName = $"Lowcode.cs";
            context.AddSource(hintName, sourceText);
        }

        private void GenerateStartup(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
            using Microsoft.Extensions.DependencyInjection;
            using {Consts.Namespace}.Model.Repositories;
            using {Consts.Namespace}.Repository;

            namespace {Consts.Namespace}
            {{
                public partial class Startup
                {{
                    // This method gets called by the runtime. Use this method to add services to the container.
                    private void ConfigureLowcodeServices(IServiceCollection services)
                    {{
            ");
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
                sourceBuilder.Append($@"services.AddScoped<I{entityName}Repository, {entityName}Repository>();");
            }
            var serviceFiles = LowcodeEngine.GetServiceFiles(context);
            foreach (var serviceFile in serviceFiles)
            {
                string serviceName = LowcodeEngine.GetFileName(serviceFile);
                sourceBuilder.Append($@"services.AddScoped<I{serviceName}Service, {serviceName}Service>();");
            }
            sourceBuilder.Append($@"
                        }}
                }}
            }}
            ");
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            string hintName = $"Startup.cs";
            context.AddSource(hintName, sourceText);
        }

        private void GenerateControllers(GeneratorExecutionContext context)
        {
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
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
                using {Consts.Namespace}.Model.Dtos;
                using {Consts.Namespace}.Model.Entities;
                using {Consts.Namespace}.Model.Repositories;
                using Microsoft.AspNetCore.Authorization;
                using Microsoft.AspNetCore.Mvc;

                namespace {Consts.Namespace}.Controllers
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

                string hintName = $"{controllerName}Controller.cs";
                context.AddSource(hintName, sourceText);
            }
        }

        private void GenerateMapper(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder();
            sourceBuilder.Append($@"
                using AutoMapper;
                using {Consts.Namespace}.Model.Dtos;
                using {Consts.Namespace}.Model.Entities;

                namespace {Consts.Namespace}.AutoMapperProfiles
                {{
                    public partial class MapperProfiles : Profile
                    {{
                        public MapperProfiles()
                        {{");
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowcodeEngine.GetEntityMetaManifest(context, entityFile);
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
            context.AddSource($"MapperProfiles.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
        private void GenerateRepositories(GeneratorExecutionContext context)
        {
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowcodeEngine.GetEntityMetaManifest(context, entityFile);
                string segmentSql = String.Join(",", entityMetaManifest.Keys);
                var repositorySourceBuilder = new StringBuilder();
                repositorySourceBuilder.Append($@"
                using Dapper;
                using {Consts.Namespace}.Model.Entities;
                using {Consts.Namespace}.Model.Repositories;
                using FewBox.Core.Persistence.Orm;
                using System;

                namespace {Consts.Namespace}.Repository
                {{
                    public partial class {entityName}Repository : Repository<{entityName}>, I{entityName}Repository
                    {{
                        public {entityName}Repository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
                        : base(""{entityName.ToLower()}"", ormSession, currentUser)
                        {{
                        }}

                        protected override string GetSaveSegmentSql()
                        {{
                            return ""{segmentSql}"";
                        }}

                        protected override string GetUpdateSegmentSql()
                        {{
                            return ""{segmentSql}"";
                        }}

                        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
                        {{
                            throw new NotImplementedException();
                        }}
                    }}
                }}");
                var repositorySourceText = SourceText.From(repositorySourceBuilder.ToString(), Encoding.UTF8);
                string repositoryHintName = $"{entityName}Repository.cs";
                context.AddSource(repositoryHintName, repositorySourceText);
                var modelSourceBuilder = new StringBuilder();
                modelSourceBuilder.Append($@"
                using {Consts.Namespace}.Model.Entities;
                using FewBox.Core.Persistence.Orm;

                namespace {Consts.Namespace}.Model.Repositories
                {{
                    public partial interface I{entityName}Repository : IRepository<{entityName}>
                    {{
                    }}
                }}");
                var modelSourceText = SourceText.From(modelSourceBuilder.ToString(), Encoding.UTF8);
                string modelHintName = $"I{entityName}Repository.cs";
                context.AddSource(modelHintName, modelSourceText);
            }
        }

        private void GenerateEntities(GeneratorExecutionContext context)
        {
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowcodeEngine.GetEntityMetaManifest(context, entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Core.Persistence.Orm;

                namespace {Consts.Namespace}.Model.Entities
                {{
                    public class {entityName} : Entity
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    sourceBuilder.AppendLine($@"public {type} {key} {{ get; set; }}");
                }
                sourceBuilder.Append($@"
                    }}
                }}");
                var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
                string hintName = $"{entityName}.cs";
                context.AddSource(hintName, sourceText);
            }
        }

        private void GenerateDtos(GeneratorExecutionContext context)
        {
            var entityFiles = LowcodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowcodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowcodeEngine.GetEntityMetaManifest(context, entityFile);
                var showSourceBuilder = new StringBuilder();
                showSourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;

                namespace {Consts.Namespace}.Model.Dtos
                {{
                    public class {entityName}Dto : EntityDto
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    showSourceBuilder.AppendLine($@"public {type} {key} {{ get; set; }}");
                }
                showSourceBuilder.Append($@"
                    }}
                }}");
                var showSourceText = SourceText.From(showSourceBuilder.ToString(), Encoding.UTF8);
                string showHintName = $"{entityName}Dto.cs";
                context.AddSource(showHintName, showSourceText);
                var persistantSourceBuilder = new StringBuilder();
                persistantSourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;

                namespace {Consts.Namespace}.Model.Dtos
                {{
                    public class {entityName}PersistantDto : EntityDto
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    persistantSourceBuilder.AppendLine($@"public {type} {key} {{ get; set; }}");
                }
                persistantSourceBuilder.Append($@"
                    }}
                }}");
                var persistantSourceText = SourceText.From(persistantSourceBuilder.ToString(), Encoding.UTF8);
                string persistantHintName = $"{entityName}PersistantDto.cs";
                context.AddSource(persistantHintName, persistantSourceText);
            }
        }
    }
}