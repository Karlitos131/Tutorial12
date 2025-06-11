using System.ComponentModel.DataAnnotations;

namespace Tutorial10.Data.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}