namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public static class MqttConstants
{
    public const string BaseTopic = Constants.ProjectName;
    public const string SystemAvailabilityTopic = $"{BaseTopic}/status";
}