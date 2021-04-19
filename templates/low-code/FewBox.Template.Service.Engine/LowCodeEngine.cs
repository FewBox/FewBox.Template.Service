using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;

namespace FewBox.Template.Service.Engine
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
            var entityMetaManifest = JsonSerializer.Deserialize<IDictionary<string, EntityMeta>>(GetEntityContent(context, entityFile));
            return entityMetaManifest;
        }
    }
}