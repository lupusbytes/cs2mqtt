using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts;

public readonly record struct SteamId64
{
    public static readonly SteamId64 None;

    public string Value { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    private SteamId64(string value)
    {
        Value = value;
        HasValue = !string.IsNullOrWhiteSpace(Value);
    }

    public static SteamId64 FromString(string? value) => value;

    public override string ToString() => Value;

    public string ToTextualString()
    {
        const ulong steamId64Base = 76561197960265728;

        if (!ulong.TryParse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var steamId64))
        {
            throw new InvalidOperationException($"The SteamId64 value '{Value}' is not a valid unsigned 64-bit integer");
        }

        if (steamId64 < steamId64Base)
        {
            throw new InvalidOperationException($"The SteamId64 value '{Value}' is less than {steamId64Base}.");
        }

        // SteamIDs follow a fairly simple format when represented textually: "STEAM_X:Y:Z," where X, Y, and Z are integers
        // X represents the "Universe" the steam account belongs to.
        // Note: older games, such as those based on GoldSrc, use universe 0, while Source games use universe 1, for the same steam account.
        // It's impossible to obtain any kind of SteamID for players in CS2 using the console.
        // This algorithm should return 1 as the universe, which is the most modern representation.
        var x = (steamId64 >> 56) & 0xFF;

        // Y is part of the ID number for the account. Y is either 0 or 1.
        var y = steamId64 & 1;

        // Z is the "account number"
        var z = (steamId64 >> 1) & 0x7FFFFFF;

        return $"STEAM_{x}:{y}:{z}";
    }

    public static implicit operator SteamId64(string? value) => string.IsNullOrWhiteSpace(value)
        ? None
        : new SteamId64(value);
}