using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

/// <summary>
/// Orders test methods by the value of their <see cref="PriorityAttribute"/>, ascending.
/// Methods without the attribute are treated as <see cref="DefaultPriority"/>.
/// Methods sharing a priority run in alphabetical order, since the order in which
/// reflection hands them to the orderer is not guaranteed to be stable.
/// <para>
/// Follows the official xUnit.net sample:
/// https://github.com/xunit/samples.xunit/tree/main/v3/TestOrderExamples/TestCaseOrdering.
/// </para>
/// </summary>
public sealed class PriorityOrderer : ITestMethodOrderer
{
    private const int DefaultPriority = 0;

    public IReadOnlyCollection<TTestMethod?> OrderTestMethods<TTestMethod>(
        IReadOnlyCollection<TTestMethod?> testMethods)
        where TTestMethod : ITestMethod
        => testMethods
            .OrderBy(testMethod => GetPriority(testMethod))
            .ThenBy(testMethod => GetMethodName(testMethod), StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static int GetPriority(ITestMethod? testMethod)
        => testMethod is IXunitTestMethod xunitTestMethod
            ? xunitTestMethod.Method.GetCustomAttribute<PriorityAttribute>()?.Priority ?? DefaultPriority
            : DefaultPriority;

    private static string? GetMethodName(ITestMethod? testMethod)
        => (testMethod as IXunitTestMethod)?.Method.Name;
}
