using Microsoft.AspNetCore.Mvc;

namespace FewBox.Template.Service.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public partial class LowCodeController : ControllerBase
    {

        [HttpGet("dryrun")]
        public string Dryrun()
        {
            FewBox.Template.Service.Domain.Debug.Dryrun();
            FewBox.Template.Service.Model.Debug.Dryrun();
            FewBox.Template.Service.Repository.Debug.Dryrun();
            return "See the debug log in console.";
        }
    }
}
