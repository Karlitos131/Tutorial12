using System.ComponentModel.DataAnnotations;

namespace Tutorial10.RestAPI.DTOs.Auth;

public class LoginDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}