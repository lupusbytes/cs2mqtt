namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal sealed class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority { get; private set; } = priority;
}