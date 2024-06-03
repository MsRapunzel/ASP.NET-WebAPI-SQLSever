using Microsoft.AspNetCore.Mvc;
using testSQLServer.Interfaces;
using testSQLServer.Models;
using static testSQLServer.Services.StaffService;

namespace testSQLServer.Controllers;

[ApiController]
[Route("[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStaf()
    {
        var result = await _staffService.GetStaffAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(int id)
    {
        try
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return UnprocessableEntity($"Error occured: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff(Staff staff)
    {
        try
        {
            var result = await _staffService.CreateStaffAsync(staff);
            return Created(nameof(CreateStaff), result);
        }
        catch(Exception ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(int id, Staff staff)
    {
        try
        {
            await _staffService.UpdateStaffAsync(id, staff);
            return Ok(await _staffService.GetStaffByIdAsync(id));
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var result = await _staffService.DeleteStaffAsync(id);

        if (!result)
        {
            return UnprocessableEntity($"Error occured: There is no Staff with id {id}");
        }

        return Ok($"Staff with id {id} was deleted successfully.");
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Staff>>> GetFilterStaff([FromQuery] string? name,
                                                             [FromQuery] string? position,
                                                             [FromQuery] string? contactEmail,
                                                             [FromQuery] string? contactNumber,
                                                             [FromQuery] int? departmentId,
                                                             [FromQuery] OrderBy? orderBy,
                                                             [FromQuery] bool descending = false)
    {
        var staffList = await _staffService.GetFilterStaffAsync(name, position, contactEmail, contactNumber, departmentId, orderBy, descending);
        return Ok(staffList);
    }
}
