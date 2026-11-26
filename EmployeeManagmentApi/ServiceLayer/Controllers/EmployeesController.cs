using EmployeeManagementApi.Models;
using EmployeeManagementApi.Service.BusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.ServiceLayer.Controllers;

[ApiController]
[Route(EmployeeConstants.EmployeesRoute)]
public sealed class EmployeesController : ControllerBase
{
    private readonly EmployeeBusinessService _businessService;

    public EmployeesController(EmployeeBusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesAsync()
    {
        var employees = await _businessService.GetEmployeesAsync();
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> SaveEmployeeAsync([FromBody] EmployeeSaveRequest request)
    {
        try
        {
            var created = await _businessService.SaveEmployeeAsync(request, null);
            return Ok(created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut(EmployeeConstants.EmployeeByIdRoute)]
    public async Task<ActionResult<EmployeeDto>> SaveEmployeeAsync(int id, [FromBody] EmployeeSaveRequest request)
    {
        try
        {
            var updated = await _businessService.SaveEmployeeAsync(request, id);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete(EmployeeConstants.EmployeeByIdRoute)]
    public async Task<IActionResult> DeleteEmployeeAsync(int id)
    {
        bool deleted = await _businessService.DeleteEmployeeAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}