using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;
using System;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace FewBox.Service.Boot
{
    public static class LowCodeEngine
    {
        public static IEnumerable<AdditionalText> GetEntityFiles(GeneratorExecutionContext context)
        {
            string entityKeyword = "Entities";
            var entityFiles = context.AdditionalFiles.Where(at => at.Path.Contains(entityKeyword));
            return entityFiles;
        }

        public static IEnumerable<AdditionalText> GetServiceFiles(GeneratorExecutionContext context)
        {
            string serviceKeyword = "Services";
            var serviceFiles = context.AdditionalFiles.Where(at => at.Path.Contains(serviceKeyword));
            return serviceFiles;
        }

        public static string GetFileName(AdditionalText entityFile)
        {
            return Path.GetFileNameWithoutExtension(entityFile.Path);
        }

        public static string GetEntityContent(GeneratorExecutionContext context, AdditionalText entityFile)
        {
            var content = entityFile.GetText(context.CancellationToken);
            return content.ToString();
        }

        public static IDictionary<string, EntityMeta> GetEntityMetaManifest(GeneratorExecutionContext context, AdditionalText entityFile)
        {
            var entityMetaManifest = System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, EntityMeta>>(GetEntityContent(context, entityFile));
            return entityMetaManifest;
        }

        public static void GenerateDebug(GeneratorExecutionContext context)
        {
            string entityFiles = String.Join(",", GetEntityFiles(context).Select(file => Path.GetFileNameWithoutExtension(file.Path)));
            string serviceFiles = String.Join(",", GetServiceFiles(context).Select(file => Path.GetFileNameWithoutExtension(file.Path)));
            var sourceBuilder = new StringBuilder();
            string @namespace = GetNamespace(context);
            sourceBuilder.Append($@"
            using System;

            namespace {@namespace}
            {{
                public static class Debug
                {{
                    public static void Dryrun()
                    {{
                        Console.WriteLine(@""----------{@namespace}----------"");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(@""[ENTITIES] {entityFiles}"");
                        Console.WriteLine(@""[SERVICES] {serviceFiles}"");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(@""-------------------------------------------------"");
                    }}
                }}
            }}");
            var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
            string hintName = $"Debug.g.cs";
            context.AddSource(hintName, sourceText);;
        }

        public static string GetNamespace(GeneratorExecutionContext context)
        {
            return context.Compilation.AssemblyName;
        }
    }
}