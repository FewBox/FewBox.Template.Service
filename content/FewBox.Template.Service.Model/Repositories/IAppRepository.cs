using FewBox.Template.Service.Model.Entities;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Template.Service.Model.Repositories
{
    public interface IAppRepository : IBaseRepository<App, Guid>
    {
    }
}