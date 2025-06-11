namespace Tutorial10.RestAPI.DTOs.Device;

public class DeviceDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TypeId { get; set; }
    public bool IsEnabled { get; set; }

    public Dictionary<string, object> AdditionalProperties { get; set; }

    public EmployeeDto? CurrentEmployee { get; set; }
}

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
}