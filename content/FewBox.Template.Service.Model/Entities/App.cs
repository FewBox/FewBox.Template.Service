using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Template.Service.Model.Entities
{
    public class App : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
   