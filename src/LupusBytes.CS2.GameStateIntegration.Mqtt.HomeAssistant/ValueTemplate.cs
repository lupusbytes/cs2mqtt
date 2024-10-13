namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public static class ValueTemplate
{
    public static string JsonPropertyValue(string propertyName)
        => $"{{{{ value_json.{propertyName} }}}}";

    public static string NestedJsonPropertyValue(string propertyName, string nestedPropertyName)
        => $"{{{{ value_json[\"{propertyName}\"].{nestedPropertyName} }}}}";
}