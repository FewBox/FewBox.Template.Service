using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Template.Service.Controllers
{
    [ApiVersion("2.0-beta1")]
    [ApiController]
    [Route("api/v{v:apiVersion}/version")]
    public class VersionBetaController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "2.0 beta";
        }
    }
}
