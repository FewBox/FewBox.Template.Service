using System;
using System.Collections.Generic;
using FewBox.Core.Web.Security;

namespace FewBox.Template.Service.Domain
{
    public class RemoteAuthenticationService : IAuthenticationService
    {
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
             throw new NotImplementedException();
        }

        public IList<string> FindRolesByMethod(string method)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(string username, string password, string userType, out object userId, out IList<string> roles)
        {
            bool isValid = false;
            Guid id = Guid.Empty;
            roles = null;
            // Todo: Add validation here.
            userId = id;
            return isValid;
        }
    }
}