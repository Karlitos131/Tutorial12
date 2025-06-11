using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Tutorial10.Data;
using Tutorial10.Data.Models;
using Tutorial10.RestAPI.DTOs.Device;

namespace Tutorial10.RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DevicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDevices()
    {
        var devices = await _context.Devices
            .Select(d => new { d.Id, d.Name })
            .ToListAsync();

        return Ok(devices);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetDeviceById(int id)
    {
        var device = await _context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees)
                .ThenInclude(de => de.Employee)
                    .ThenInclude(e => e.Person)
            .Where(d => d.Id == id)
            .Select(d => new DeviceDetailsDto
            {
                Id = d.Id,
                Name = d.Name,
                DeviceTypeName = d.DeviceType.Name,
                IsEnabled = d.IsEnabled,
                AdditionalProperties = d.AdditionalProperties,
                CurrentEmployee = d.DeviceEmployees
                    .Where(de => de.ReturnDate == null)
                    .Select(de => new EmployeeDto
                    {
                        Id = de.Employee.Id,
                        FullName = de.Employee.Person.FirstName + " " + de.Employee.Person.LastName
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (device == null)
            return NotFound();

        return Ok(device);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDevice([FromBody] DeviceCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceType = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (deviceType == null)
            return BadRequest("Invalid device type.");

        var device = new Device
        {
            Name = dto.Name,
            IsEnabled = dto.IsEnabled,
            AdditionalProperties = System.Text.Json.JsonSerializer.Serialize(dto.AdditionalProperties),
            DeviceTypeId = deviceType.Id
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDevice(int id, [FromBody] DeviceCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (device == null)
            return NotFound();

        var deviceType = await _context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
        if (deviceType == null)
            return BadRequest("Invalid device type.");

        device.Name = dto.Name;
        device.IsEnabled = dto.IsEnabled;
        device.AdditionalProperties = System.Text.Json.JsonSerializer.Serialize(dto.AdditionalProperties);
        device.DeviceTypeId = deviceType.Id;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
            return NotFound();

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}