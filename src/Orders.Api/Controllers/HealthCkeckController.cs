using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Orders.Api.Controllers
{
    [Route("api/HealthCkeck")]
    public class HealthCkeckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HealthCkeck()
        {
            return await Task.FromResult(Ok("HeelLo"));
        }
    }
}
