using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tutorial10.Data;
using Tutorial10.Data.Models;

namespace Tutorial10.RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] Account account)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
            return BadRequest("Username already exists.");
        var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{12,}$";
        if (!Regex.IsMatch(account.PasswordHash, passwordPattern))
            return BadRequest("Password does not meet security requirements.");

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = account.Id }, null);
    }
}