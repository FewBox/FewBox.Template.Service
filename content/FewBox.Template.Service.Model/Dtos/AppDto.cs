using FewBox.Core.Web.Dto;
using System;

namespace FewBox.Template.Service.Model.Dtos
{
    public class AppDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}