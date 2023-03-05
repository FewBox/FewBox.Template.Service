using System.Collections.Generic;
using FewBox.Template.Service.Model.Entities;

namespace FewBox.Template.Service.Model.Repositories
{
    public partial interface IAppRepository
    {
        IEnumerable<App> FindAllByName(string name);
    }
}