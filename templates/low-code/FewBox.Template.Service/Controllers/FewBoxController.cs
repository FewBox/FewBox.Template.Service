using FewBox.Core.Web.Dto;
using FewBox.Template.Service.Model.Dtos;
using FewBox.Template.Service.Model.Services;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Template.Service.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class FewBoxController : ControllerBase
    {
        private  IFewBoxService FewBoxService { get; set; }

        public FewBoxController(IFewBoxService fewBoxService)
        {
            this.FewBoxService = fewBoxService;
        }

        [HttpGet]
        public PayloadResponseDto<AuthorDto> Get()
        {
            Model.Entities.App app = new Model.Entities.App();
            app.Debug();
            return new PayloadResponseDto<AuthorDto>
            {
                Payload =this.FewBoxService.GetAuthor()
            };
        }
    }
}
