using System;
using System.Collections.Generic;
using FewBox.Template.Service.Model.Entities;
using Dapper;

namespace FewBox.Template.Service.Repository
{
    public partial class AppRepository
    {
        public IEnumerable<App> FindAllByName(string name)
        {
            return this.UnitOfWork.Connection.Query<App>($"select * from {TableName} where Name=@Name", new { Name = name });
        }
    }
}