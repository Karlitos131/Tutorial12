using System.ComponentModel.DataAnnotations;

namespace Tutorial10.Data.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [RegularExpression("^[^\\d].*", ErrorMessage = "Username cannot start with a digit.")]
    public string Username { get; set; }

    [Required]
    [MinLength(12)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{12,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character and be at least 12 characters long.")]
    public string PasswordHash { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public int RoleId { get; set; }

    public Employee Employee { get; set; }
    public Role Role { get; set; }
}