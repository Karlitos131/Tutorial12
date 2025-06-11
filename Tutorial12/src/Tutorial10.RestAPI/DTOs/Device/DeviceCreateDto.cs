using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json;

namespace Tutorial10.RestAPI.DTOs.Device;

public class DeviceCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public bool IsEnabled { get; set; }

    [Required]
    public int TypeId { get; set; }

    [Required]
    public Dictionary<string, JsonElement> AdditionalProperties { get; set; }
}