using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using FewBox.Template.Service.Engine;

namespace FewBox.Template.Service.Repository.FewBox
{
    [Generator]
    class FewBoxRepositoryGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            this.GenerateRepository(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            System.Diagnostics.Debugger.Launch();
        }

        private void GenerateRepository(GeneratorExecutionContext context)
        {
            var entityFiles = LowCodeEngine.GetEntityFiles(context);
            foreach (var entityFile in entityFiles)
            {
                string entityName = LowCodeEngine.GetFileName(entityFile);
                var entityMetaManifest = LowCodeEngine.GetEntityMetaManifest(context, entityFile);
                string segmentSql = String.Join(',', entityMetaManifest.Keys);
                var sourceBuilder = new StringBuilder();
                sourceBuilder.Append($@"
                using Dapper;
                using FewBox.Template.Service.Model.Entities;
                using FewBox.Template.Service.Model.Repositories;
                using FewBox.Core.Persistence.Orm;
                using System;

                namespace FewBox.Template.Service.Repository
                {{
                    public class {entityName}Repository : Repository<{entityName}>, I{entityName}Repository
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
                var sourceText = SourceText.From(sourceBuilder.ToString(), Encoding.UTF8);
                string hintName = $"{entityName}Repository.cs";
                context.AddSource(hintName, sourceText);
            }
        }
    }
}