using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiniMicroservice.API.Controllers;

[Route("api/[controller]"), ApiController, Authorize]
public class PersonsController : ControllerBase
{
    private readonly List<string> _list = ["Bahram Bayramzade", "Nadir Zamanov", "Gulya Abbasova", "Kenan Aliyev"];

    [HttpGet("get-all")]
    public async Task<ActionResult<List<string>>> GetAll()
    {
        return await Task.FromResult(Ok(_list));
    }
}