using Dapper;
using FewBox.Template.Service.Model.Entities;
using FewBox.Template.Service.Model.Repositories;
using FewBox.Core.Persistence.Orm;
using System;

namespace FewBox.Template.Service.Repository
{
    public class AppRepository : Repository<App>, IAppRepository
    {
        public AppRepository(IOrmSession ormSession, ICurrentUser<Guid> currentUser)
        : base("app", ormSession, currentUser)
        {
        }

        protected override string GetSaveSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateSegmentSql()
        {
            return "Name";
        }

        protected override string GetUpdateWithUniqueKeyWhereSegmentSql()
        {
            throw new NotImplementedException();
        }
    }
}