using System.Diagnostics.CodeAnalysis;

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

    public override string ToString() => Value;

    public static SteamId64 FromString(string? value) => value;

    public static implicit operator SteamId64(string? value) => string.IsNullOrWhiteSpace(value)
        ? None
        : new SteamId64(value);
}