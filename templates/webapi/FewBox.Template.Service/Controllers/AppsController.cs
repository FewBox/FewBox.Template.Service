using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Entities;
using FewBox.Template.Service.Model.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Template.Service.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class AppsController : ResourcesController<IAppRepository, App, AppDto, AppPersistantDto>
    {
        public AppsController(IAppRepository appRepository, IMapper mapper) : base(appRepository, mapper)
        {
            // SQLite ID must be Upcase.
        }
    }
}
