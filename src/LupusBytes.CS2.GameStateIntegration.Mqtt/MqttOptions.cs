namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class MqttOptions
{
    public const string Section = "Mqtt";

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 1883;

    public bool UseTls { get; set; }
}