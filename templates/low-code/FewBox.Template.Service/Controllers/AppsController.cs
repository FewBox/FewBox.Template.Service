using System;
using System.Collections.Generic;
using FewBox.Core.Web.Dto;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Template.Service.Controllers
{
    public partial class AppsController
    {
        [HttpGet("filter")]
        public PayloadResponseDto<IEnumerable<AppDto>> Filter([FromQuery] string name)
        {
            return new PayloadResponseDto<IEnumerable<AppDto>>
            {
                Payload = this.Mapper.Map<IEnumerable<App>, IEnumerable<AppDto>>(this.Repository.FindAllByName(name))
            };
        }
    }
}
