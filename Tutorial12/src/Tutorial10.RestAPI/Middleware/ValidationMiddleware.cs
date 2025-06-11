using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tutorial10.RestAPI.Validation;

namespace Tutorial10.RestAPI.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;
    private readonly ValidationLoader _loader;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger, ValidationLoader loader)
    {
        _next = next;
        _logger = logger;
        _loader = loader;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if ((context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put) &&
            context.Request.Path.StartsWithSegments("/api/devices"))
        {
            _logger.LogInformation("ValidationMiddleware triggered for {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("deviceTypeId", out var typeProp) || !typeProp.TryGetInt32(out var deviceTypeId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("deviceTypeId is missing or invalid.");
                return;
            }

            if (!root.TryGetProperty("additionalProperties", out var additional) || additional.ValueKind != JsonValueKind.Object)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("additionalProperties object is missing or invalid.");
                return;
            }

            var rules = _loader.GetRulesForDeviceType(deviceTypeId);
            var errors = new List<string>();

            foreach (var rule in rules)
            {
                if (!string.IsNullOrEmpty(rule.PreRequestName) &&
                    (!root.TryGetProperty(rule.PreRequestName, out var preProp) || preProp.GetString() != rule.PreRequestValue))
                {
                    continue;
                }

                if (!additional.TryGetProperty(rule.Name, out var prop))
                {
                    if (rule.Required)
                        errors.Add($"{rule.Name} is required.");
                    continue;
                }

                try
                {
                    switch (rule.Type.ToLower())
                    {
                        case "string":
                            var strVal = prop.GetString();
                            if (rule.Required && string.IsNullOrEmpty(strVal))
                                errors.Add($"{rule.Name} is required and cannot be empty.");
                            if (!string.IsNullOrEmpty(rule.Regex) && !System.Text.RegularExpressions.Regex.IsMatch(strVal ?? "", rule.Regex))
                                errors.Add($"{rule.Name} does not match required format.");
                            break;
                        case "int":
                            if (!prop.TryGetInt32(out var intVal))
                                errors.Add($"{rule.Name} must be an integer.");
                            else
                            {
                                if (rule.Min.HasValue && intVal < rule.Min.Value)
                                    errors.Add($"{rule.Name} must be at least {rule.Min.Value}.");
                                if (rule.Max.HasValue && intVal > rule.Max.Value)
                                    errors.Add($"{rule.Name} must be at most {rule.Max.Value}.");
                            }
                            break;
                        case "bool":
                            if (prop.ValueKind != JsonValueKind.True && prop.ValueKind != JsonValueKind.False)
                                errors.Add($"{rule.Name} must be a boolean.");
                            break;
                        default:
                            errors.Add($"{rule.Name} has unknown type '{rule.Type}'.");
                            break;
                    }
                }
                catch
                {
                    errors.Add($"{rule.Name} is invalid or in wrong format.");
                }
            }

            if (errors.Count > 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors }));
                return;
            }
        }

        await _next(context);
    }
}