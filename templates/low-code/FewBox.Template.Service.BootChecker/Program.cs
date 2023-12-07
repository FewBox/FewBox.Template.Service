using System;

namespace FewBox.Template.Service.BootChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            FewBox.Template.Service.Domain.Debug.Dryrun();
            FewBox.Template.Service.Model.Debug.Dryrun();
            FewBox.Template.Service.Repository.Debug.Dryrun();
            Console.WriteLine("Finish!");
            Console.ReadKey();
        }
    }
}
