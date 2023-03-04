using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.CodeAnalysis;

namespace FewBox.Template.Service.Boot
{
    public static class LowcodeEngine
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
    }
}