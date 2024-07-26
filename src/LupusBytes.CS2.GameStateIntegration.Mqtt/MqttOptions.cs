namespace LupusBytes.CS2.GameStateIntegration.Mqtt;

public class MqttOptions
{
    /// <summary>
    /// Key for getting the <see cref="MqttOptions"/> subsection from the configuration.
    /// </summary>
    public const string Section = "Mqtt";

    /// <summary>
    /// The hostname or IP address of the MQTT broker.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// The port number to use when connecting to the MQTT broker.
    /// </summary>
    public int Port { get; set; } = 1883;

    /// <summary>
    /// The client ID to use when connecting to the MQTT broker.
    /// </summary>
    public string ClientId { get; set; } = Constants.ProjectName;

    /// <summary>
    /// Whether to use SSL/TLS when connecting to the MQTT broker. The default setting is <see langword="false" />.
    /// </summary>
    public bool UseTls { get; set; }
}