using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using FewBox.Template.Service.Boot;

namespace FewBox.Template.Service.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class LowcodeController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            return Lowcode.GetJsonFiles();
        }
    }
}
