using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FewBox.Service.Boot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FewBox.Template.Service.Model.Boot
{
    [Generator]
    class FewBoxBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            LowCodeEngine.GenerateDebug(context);
            this.GenerateDtos(context);
            this.GenerateEntities(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            Debug.WriteLine("Initalize code generator");
        }

        private void GenerateEntities(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Core.Persistence.Orm;
                namespace {LowCodeEngine.GetNamespace(context)}.Entities
                {{
                    public partial class {entityName} : Entity
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    sourceBuilder.AppendLine($@"
                            public {type} {key} {{ get; set; }}
                    ");
                }
                if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.LowCode_IsDebug", out var emitLoggingSwitch))
                {
                    bool isDebug = emitLoggingSwitch.Equals("true", StringComparison.OrdinalIgnoreCase);
                    if (isDebug)
                    {
                        var plainTextBytes = Encoding.UTF8.GetBytes(LowCodeEngine.GetEntityContent(context, entityFile));
                        string debug = Convert.ToBase64String(plainTextBytes);
                        sourceBuilder.Append($@"
                                public void Debug()
                                {{
                                    System.Console.WriteLine(""{debug}"");
                                }}
                        ");
                    }
                }
                sourceBuilder.Append($@"}}
                }}");
                context.AddSource($"{entityName}.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
                var modelSourceBuilder = new StringBuilder();
                modelSourceBuilder.Append($@"
                using {LowCodeEngine.GetNamespace(context)}.Entities;
                using FewBox.Core.Persistence.Orm;

                namespace {LowCodeEngine.GetNamespace(context)}.Repositories
                {{
                    public partial interface I{entityName}Repository : IRepository<{entityName}>
                    {{
                    }}
                }}");
                var modelSourceText = SourceText.From(modelSourceBuilder.ToString(), Encoding.UTF8);
                string modelHintName = $"I{entityName}Repository.g.cs";
                context.AddSource(modelHintName, modelSourceText);
            }
        }

        private void GenerateDtos(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                var showSourceBuilder = new StringBuilder();
                showSourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;

                namespace {LowCodeEngine.GetNamespace(context)}.Dtos
                {{
                    public partial class {entityName}Dto : EntityDto
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
                string showHintName = $"{entityName}Dto.g.cs";
                context.AddSource(showHintName, showSourceText);
                var persistantSourceBuilder = new StringBuilder();
                persistantSourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;

                namespace {LowCodeEngine.GetNamespace(context)}.Dtos
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
                string persistantHintName = $"{entityName}PersistantDto.g.cs";
                context.AddSource(persistantHintName, persistantSourceText);
            }
        }

    }
}