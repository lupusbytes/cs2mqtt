using Atc.Test.Customizations;
using NSubstitute;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.Tests.Customizations;

[AutoRegister]
internal sealed class MqttOptionsProviderCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<IMqttOptionsProvider>(composer => composer
            .FromFactory(() =>
            {
                var provider = Substitute.For<IMqttOptionsProvider>();
                var options = fixture.Create<MqttOptions>();
                provider.GetOptionsAsync(Arg.Any<CancellationToken>()).Returns(options);
                return provider;
            })
            .OmitAutoProperties());
    }
}
