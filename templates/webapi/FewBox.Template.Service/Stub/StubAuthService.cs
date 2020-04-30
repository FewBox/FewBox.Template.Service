using System.Collections.Generic;
using FewBox.Core.Web.Security;

namespace FewBox.Template.Service.Stub
{
    public class StubAuthService : IAuthService
    {
        public bool DoesUserHavePermission(string service, string controller, string action, IList<string> roles)
        {
            return true;
        }

        public bool DoesUserHavePermission(string method, IList<string> roles)
        {
            return true;
        }
    }
}