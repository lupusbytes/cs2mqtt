using Atc.Test.Customizations;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.Customizations;

[AutoRegister]
internal sealed class MqttOptionsCustomization : ICustomization
{
    private static readonly string[] ProtocolVersions = ["5.0.0", "3.1.1", "3.1.0"];

    public void Customize(IFixture fixture)
    {
        fixture.Customize<MqttOptions>(composer => composer.With(
            x => x.ProtocolVersion,
            ProtocolVersions[fixture.Create<int>() % ProtocolVersions.Length]));
    }
}