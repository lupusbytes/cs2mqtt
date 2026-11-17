namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

/// <summary>
/// Assigns the execution order of a test case within its class, when the class
/// is ordered by <see cref="PriorityOrderer"/>. Lower values run first.
/// </summary>
/// <param name="priority">The execution order of the test case. Lower values run first.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PriorityAttribute(int priority) : Attribute
{
    public int Priority => priority;
}
