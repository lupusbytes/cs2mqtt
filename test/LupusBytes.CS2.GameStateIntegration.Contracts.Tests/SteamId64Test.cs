namespace LupusBytes.CS2.GameStateIntegration.Contracts.Tests;

public class SteamId64Test
{
    [Theory]
    [InlineData("76561197960287930", "STEAM_1:0:11101")]
    [InlineData("76561197981496355", "STEAM_1:1:10615313")]
    [InlineData("76561197992190057", "STEAM_1:1:15962164")]
    [InlineData("76561197972686839", "STEAM_1:1:6210555")]
    public void ToTextualString_should_return_textual_SteamID_using_universe_1(string steamId64, string steamId)
        => SteamId64.FromString(steamId64).ToTextualString().Should().Be(steamId);

    [Theory]
    [InlineData("Foo")]
    [InlineData("STEAM_1:1:10615313")]
    [InlineData("-9223372036854775808")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("12345678")]
    [InlineData("76561197960265727")]
    public void ToTextualString_should_throw_invalid_operation(string steamId64)
    {
        // Arrange
        var sut = SteamId64.FromString(steamId64);

        // Act & Assert
        sut.Invoking(x => x.ToTextualString())
            .Should()
            .Throw<InvalidOperationException>();
    }
}