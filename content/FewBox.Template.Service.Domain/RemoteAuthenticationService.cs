using System;
using System.Collections.Generic;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Security;
using FewBox.Template.Service.Model.Configs;
using Microsoft.AspNetCore.Http;

namespace FewBox.Template.Service.Domain
{
    public class RemoteAuthenticationService : IAuthenticationService
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private SecurityConfig SecurityConfig { get; set; }

        public RemoteAuthenticationService(IHttpContextAccessor httpContextAccessor, SecurityConfig securityConfig)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.SecurityConfig = securityConfig;
        }
        public IList<string> FindRolesByControllerAndAction(string controller, string action)
        {
            var headers = new List<Header>();
            foreach(var header in this.HttpContextAccessor.HttpContext.Request.Headers)
            {
                headers.Add(new Header { Key = header.Key, Value=header.Value });
            }
            var response = RestfulUtility.Get<PayloadResponseDto<IList<string>>>($"{this.SecurityConfig.Protocol}://{this.SecurityConfig.Host}:{this.SecurityConfig.Port}/api/security/{controller}/{action}", headers);
            return response.Payload;
        }

        public IList<string> FindRolesByMethod(string method)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(string username, string password, string userType, out object userId, out IList<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}