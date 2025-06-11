namespace Tutorial10.RestAPI.Validation;

public class ValidationRuleSet
{
    public int DeviceTypeId { get; set; }
    public List<ValidationRule> Rules { get; set; }
}