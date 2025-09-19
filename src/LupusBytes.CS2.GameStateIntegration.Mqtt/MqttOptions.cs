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
    /// The MQTT protocol version to use when connection to the MQTT broker.
    /// The version should be in SemVer format, such as "5.0.0", "3.1.1", or "3.1.0".
    /// </summary>
    public string ProtocolVersion { get; set; } = "5.0.0";

    /// <summary>
    /// Whether to use SSL/TLS when connecting to the MQTT broker. The default setting is <see langword="false" />.
    /// </summary>
    public bool UseTls { get; set; }

    /// <summary>
    /// The username to use when connecting to the MQTT broker.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// The password to use when connecting to the MQTT broker.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// The client ID to use when connecting to the MQTT broker.
    /// </summary>
    public string ClientId { get; set; } = Constants.ProjectName;

    /// <summary>
    /// Function that determines the delay between reconnect attempts, given the current attempt number.
    /// </summary>
    internal Func<int, TimeSpan> ReconnectDelayProvider { get; set; } = attempt => TimeSpan.FromSeconds(Math.Pow(2.5, attempt));
}