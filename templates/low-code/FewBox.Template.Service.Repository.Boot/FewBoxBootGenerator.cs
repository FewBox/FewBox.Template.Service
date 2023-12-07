using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FewBox.Service.Boot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FewBox.Template.Service.Repository.Boot
{
    [Generator]
    class FewBoxBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            LowCodeEngine.GenerateDebug(context);
            this.GenerateRepositories(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            Debug.WriteLine("Initalize code generator");
        }

        private void GenerateRepositories(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                string segmentSql = String.Join(",", entityMetaManifest.Keys);
                var repositorySourceBuilder = new StringBuilder();
                repositorySourceBuilder.Append($@"
                using Dapper;
                using {LowCodeEngine.GetNamespace(context).Replace("Repository", "Model")}.Entities;
                using {LowCodeEngine.GetNamespace(context).Replace("Repository", "Model")}.Repositories;
                using FewBox.Core.Persistence.Orm;
                using System;

                namespace {LowCodeEngine.GetNamespace(context)}
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
                string repositoryHintName = $"{entityName}Repository.g.cs";
                context.AddSource(repositoryHintName, repositorySourceText);
            }
        }

    }
}