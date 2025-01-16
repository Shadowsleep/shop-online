using Microsoft.AspNetCore.Mvc;

namespace Notifications.Api.Controllers
{
    [ApiController]
    [Route("api/HealthCkeck")]
    public class HealthCkeckController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> HealthCkeck()
        {
            return await Task.FromResult(Ok("teste realizado"));
        }
    }
}
