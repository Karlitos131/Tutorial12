using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Tutorial10.Data;
using Tutorial10.Data.Models;
using Tutorial10.RestAPI.DTOs.Device;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Tutorial10.RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDevices()
    {
        _logger.LogInformation("Fetching all devices.");
        var devices = await _context.Devices
            .Select(d => new { d.Id, d.Name })
            .ToListAsync();

        return Ok(devices);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetDeviceById(int id)
    {
        _logger.LogInformation("Fetching device with ID {Id}", id);
        var deviceEntity = await _context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees)
                .ThenInclude(de => de.Employee)
                    .ThenInclude(e => e.Person)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (deviceEntity == null)
            return NotFound();

        var device = new DeviceDetailsDto
        {
            Id = deviceEntity.Id,
            Name = deviceEntity.Name,
            TypeId = deviceEntity.DeviceTypeId ?? 0,
            IsEnabled = deviceEntity.IsEnabled,
            AdditionalProperties = JsonSerializer.Deserialize<Dictionary<string, object>>(deviceEntity.AdditionalProperties) ?? new Dictionary<string, object>(),
            CurrentEmployee = deviceEntity.DeviceEmployees
                .Where(de => de.ReturnDate == null)
                .Select(de => new EmployeeDto
                {
                    Id = de.Employee.Id,
                    FullName = de.Employee.Person.FirstName + " " + de.Employee.Person.LastName
                })
                .FirstOrDefault()
        };

        return Ok(device);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateDevice([FromBody] DeviceCreateDto dto)
    {
        _logger.LogInformation("Creating new device: {Name}", dto.Name);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var deviceTypeExists = await _context.DeviceTypes.AnyAsync(dt => dt.Id == dto.TypeId);
        if (!deviceTypeExists)
            return BadRequest("Invalid device type.");

        var device = new Device
        {
            Name = dto.Name,
            IsEnabled = dto.IsEnabled,
            AdditionalProperties = JsonSerializer.Serialize(dto.AdditionalProperties),
            DeviceTypeId = dto.TypeId
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDevice(int id, [FromBody] DeviceCreateDto dto)
    {
        _logger.LogInformation("Updating device ID {Id}", id);
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var device = await _context.Devices.FirstOrDefaultAsync(d => d.Id == id);
        if (device == null)
            return NotFound();

        var deviceTypeExists = await _context.DeviceTypes.AnyAsync(dt => dt.Id == dto.TypeId);
        if (!deviceTypeExists)
            return BadRequest("Invalid device type.");

        device.Name = dto.Name;
        device.IsEnabled = dto.IsEnabled;
        device.AdditionalProperties = JsonSerializer.Serialize(dto.AdditionalProperties);
        device.DeviceTypeId = dto.TypeId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDevice(int id)
    {
        _logger.LogInformation("Deleting device ID {Id}", id);
        var device = await _context.Devices.FindAsync(id);
        if (device == null)
            return NotFound();

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}