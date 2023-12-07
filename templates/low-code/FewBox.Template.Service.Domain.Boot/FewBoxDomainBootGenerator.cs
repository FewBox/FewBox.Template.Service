using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FewBox.Service.Boot;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FewBox.Template.Service.Domain.Boot
{
    [Generator]
    public class FewBoxDomainBootGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            LowCodeEngine.GenerateDebug(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            Debugger.Launch();
            Debug.WriteLine("Initalize code generator");
        }
    }
}