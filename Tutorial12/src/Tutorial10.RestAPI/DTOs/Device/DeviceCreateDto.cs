using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Tutorial10.RestAPI.DTOs.Device;

public class DeviceCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string DeviceTypeName { get; set; }

    [Required]
    public bool IsEnabled { get; set; }

    [Required]
    public Dictionary<string, object> AdditionalProperties { get; set; }
}