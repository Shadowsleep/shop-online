using Microsoft.AspNetCore.Mvc;

namespace Payments.Api.Controllers
{
    [ApiController]
    [Route("HealthCkeck")]
    public class HealthCkeckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HealthCkeck()
        {
            return await Task.FromResult(Ok());
        }
    }
}
