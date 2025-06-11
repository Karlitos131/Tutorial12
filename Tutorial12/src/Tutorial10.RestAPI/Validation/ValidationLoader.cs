using System.Text.Json;

namespace Tutorial10.RestAPI.Validation;

public class ValidationLoader
{
    private readonly List<ValidationRuleSet> _rules;

    public ValidationLoader(IConfiguration configuration)
    {
        var path = configuration["ValidationRules:Path"];
        if (string.IsNullOrEmpty(path))
        {
            throw new InvalidOperationException("Validation rules path is not configured.");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Validation rules file not found.", path);
        }

        var json = File.ReadAllText(path);
        _rules = JsonSerializer.Deserialize<List<ValidationRuleSet>>(json)
                 ?? throw new InvalidOperationException("Validation rules could not be deserialized.");
    }

    public List<ValidationRuleSet> GetAll() => _rules;

    public List<ValidationRule> GetRulesForDeviceType(int deviceTypeId) =>
        _rules.FirstOrDefault(r => r.DeviceTypeId == deviceTypeId)?.Rules ?? new List<ValidationRule>();
}