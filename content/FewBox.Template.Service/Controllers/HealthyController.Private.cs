using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace FewBox.Template.Service.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy="JWTRole_ControllerAction")]
    public class HealthyController : ControllerBase
    {
        private IAppService AppService { get; set; }

        public HealthyController(IAppService AppService)
        {
            this.AppService = AppService;
        }

        [HttpGet]
        public PayloadResponseDto<HealthyDto> Get()
        {
            return new PayloadResponseDto<HealthyDto>
            {
                Payload = this.AppService.GetHealtyInfo()
            };
        }
    }
}
