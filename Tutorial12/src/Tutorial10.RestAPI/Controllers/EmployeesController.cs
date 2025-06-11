using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutorial10.Data;
using Tutorial10.RestAPI.DTOs.Device;
using Microsoft.AspNetCore.Authorization;

namespace Tutorial10.RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.Person)
            .Select(e => new
            {
                e.Id,
                FullName = e.Person.FirstName + " " + e.Person.LastName
            })
            .ToListAsync();

        return Ok(employees);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Position)
            .Include(e => e.Person)
            .Where(e => e.Id == id)
            .Select(e => new
            {
                e.Id,
                e.Salary,
                e.HireDate,
                Position = new
                {
                    e.Position.Id,
                    e.Position.Name
                },
                Person = new
                {
                    e.Person.FirstName,
                    e.Person.LastName,
                    e.Person.PassportNumber,
                    e.Person.Email,
                    e.Person.PhoneNumber
                }
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }
}