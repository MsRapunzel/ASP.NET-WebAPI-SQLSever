using Microsoft.AspNetCore.Mvc;
using testSQLServer.Interfaces;

namespace testSQLServer.Controllers;

[ApiController]
[Route("[controller]")]
public class SpecimensController : ControllerBase
{
    private readonly ISpecimensService _specimensService;

    public SpecimensController(ISpecimensService specimensService)
    {
        _specimensService = specimensService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSpecimens()
    {
        var result = await _specimensService.GetAllSpecimensData();
        return Ok(result);
    }
}