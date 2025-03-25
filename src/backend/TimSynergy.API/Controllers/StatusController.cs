using Microsoft.AspNetCore.Mvc;

namespace TimSynergy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Status = "API is running", Version = "1.0.0" });
        }
    }
}
