namespace CleanAuth.API.Controllers;

[Route("api/[controller]"), ApiController, Authorize]
public class OldDaysController : ControllerBase
{
    [HttpGet("back-to-2022")]
    public IActionResult BackTo2022()
    {
        return Ok("Теплом так веет от старых комнат...");
    }
}