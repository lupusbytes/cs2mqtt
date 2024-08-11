using System.Reflection;

namespace LupusBytes.CS2.GameStateIntegration;

public static class Constants
{
    public const string ProjectName = "cs2mqtt";

    public static string Version { get; } = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion ?? "Unknown";
}