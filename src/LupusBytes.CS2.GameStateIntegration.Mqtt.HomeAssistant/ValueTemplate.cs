namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public static class ValueTemplate
{
    public static string JsonPropertyValue(string propertyName)
        => $"{{{{ value_json.{propertyName} }}}}";

    public static string JsonPropertyValueWithFallback(string property, string fallback) =>
        $$$"""
           {% if value_json.{{{property}}} is not none %}
             {{ value_json.{{{property}}} }}
           {% else %}
             {{{fallback}}}
           {% endif %}
           """;

    public static string NestedJsonPropertyValue(string propertyName, string nestedPropertyName)
        => $"{{{{ value_json[\"{propertyName}\"].{nestedPropertyName} }}}}";
}