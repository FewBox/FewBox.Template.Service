using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using FewBox.Template.Service.Engine;

namespace FewBox.Template.Service.Model.FewBox
{
    [Generator]
    class FewBoxModelGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            this.GenerateDemo(context);
            this.GenerateEntity(context);
            this.GenerateDto(context);
            this.GeneratePersistantDto(context);
            this.GenerateRepositoryInterface(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            System.Diagnostics.Debugger.Launch();
        }

        private void GenerateDemo(GeneratorExecutionContext context)
        {
            context.AddSource("GeneratedClass.cs", SourceText.From(@"
            using System;
            namespace GeneratedNamespace
            {
                public class GeneratedClass
                {
                    public static void GeneratedMethod()
                    {
                        Console.WriteLine(""Hello World!"");
                    }
                }
            }", Encoding.UTF8));
        }

        private void GenerateEntity(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Core.Persistence.Orm;
                namespace FewBox.Template.Service.Model.Entities
                {{
                    public class {entityName} : Entity
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
                    isDebug = true;
                    if (isDebug)
                    {
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(LowCodeEngine.GetEntityContent(context, entityFile));
                        string debug = System.Convert.ToBase64String(plainTextBytes);
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
                context.AddSource($"{entityName}.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        private void GenerateDto(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;
                namespace FewBox.Template.Service.Model.Dtos
                {{
                    public class {entityName}Dto : EntityDto
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    sourceBuilder.AppendLine($@"
                            public {type} {key} {{ get; set; }}
                    ");
                }
                sourceBuilder.Append($@"}}
                }}");
                context.AddSource($"{entityName}Dto.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        private void GeneratePersistantDto(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Core.Web.Dto;
                namespace FewBox.Template.Service.Model.Dtos
                {{
                    public class {entityName}PersistantDto
                    {{");
                foreach (string key in entityMetaManifest.Keys)
                {
                    string type = entityMetaManifest[key].Type;
                    sourceBuilder.AppendLine($@"
                            public {type} {key} {{ get; set; }}
                    ");
                }
                sourceBuilder.Append($@"}}
                }}");
                context.AddSource($"{entityName}PersistantDto.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }
        }

        private void GenerateRepositoryInterface(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using FewBox.Template.Service.Model.Entities;
                using FewBox.Core.Persistence.Orm;

                namespace FewBox.Template.Service.Model.Repositories
                {{
                    public interface I{entityName}Repository : IRepository<{entityName}>
                    {{
                    }}
                }}");
                var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
                string hintName = $"I{entityName}Repository.cs";
                context.AddSource(hintName, sourceText);
            }
        }
    }
}