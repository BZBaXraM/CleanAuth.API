namespace CleanAuth.API.Controllers;

/// <summary>
/// This controller is a fun addition to the API that provides a nostalgic endpoint about the old days of 2022. It is not meant for any real functionality and is just for entertainment purposes. The endpoint is protected by authorization, so only authenticated users can access it.
/// </summary>
[Route("api/[controller]"), ApiController, Authorize]
public class OldDaysController : ControllerBase
{
    /// <summary>
    /// Returns a nostalgic message about the old days of 2022, when the world was different and simpler. This endpoint is just for fun and has no real functionality.
    /// </summary>
    /// <returns></returns>
    [HttpGet("back-to-2022")]
    public IActionResult BackTo2022()
    {
        return Ok("Теплом так веет от старых комнат...");
    }
}