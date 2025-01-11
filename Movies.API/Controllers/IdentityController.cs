using Microsoft.AspNetCore.Mvc;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetClaims()
        {
            return Ok(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}