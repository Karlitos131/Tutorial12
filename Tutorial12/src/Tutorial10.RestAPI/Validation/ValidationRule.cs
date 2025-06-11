namespace Tutorial10.RestAPI.Validation;

public class ValidationRule
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool Required { get; set; }
    public string Regex { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public string PreRequestName { get; set; }
    public string PreRequestValue { get; set; }
}