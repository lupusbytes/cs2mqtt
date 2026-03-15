namespace LupusBytes.CS2.GameStateIntegration.Contracts.Tests;

public class SteamId64Test
{
    [Theory]
    [InlineData("76561197960287930", "STEAM_1:0:11101")]
    [InlineData("76561197981496355", "STEAM_1:1:10615313")]
    [InlineData("76561197992190057", "STEAM_1:1:15962164")]
    [InlineData("76561197972686839", "STEAM_1:1:6210555")]
    [InlineData("76561198228701183", "STEAM_1:1:134217727")] // Z = 2^27 - 1 (max value fitting in 27 bits)
    [InlineData("76561198228701184", "STEAM_1:0:134217728")] // Z = 2^27 (first value requiring bit 27)
    [InlineData("76561198228701185", "STEAM_1:1:134217728")] // Z = 2^27 with Y = 1
    [InlineData("76561198497136640", "STEAM_1:0:268435456")] // Z = 2^28 (first value requiring bit 28)
    [InlineData("76561199034007552", "STEAM_1:0:536870912")] // Z = 2^29 (first value requiring bit 29)
    [InlineData("76561200107749376", "STEAM_1:0:1073741824")] // Z = 2^30 (first value requiring bit 30)
    [InlineData("76561202255233023", "STEAM_1:1:2147483647")] // Z = 2^31 - 1 (max account number, highest possible SteamID64 before breaking the textual representation)
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