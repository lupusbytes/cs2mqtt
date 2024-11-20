using Xunit.Abstractions;
using Xunit.Sdk;

namespace LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests.Helpers;

internal sealed class PriorityOrderer : ITestCaseOrderer
{
    public const string Assembly = "LupusBytes.CS2.GameStateIntegration.Api.EndToEnd.Tests";
    public const string Name = $"{Assembly}.Helpers.{nameof(PriorityOrderer)}";

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var assemblyName = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
        var sortedMethods = new SortedDictionary<int, List<TTestCase>>();
        foreach (var testCase in testCases)
        {
            var priority = testCase.TestMethod.Method
                .GetCustomAttributes(assemblyName)
                .FirstOrDefault()
                ?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? 0;

            GetOrCreate(sortedMethods, priority).Add(testCase);
        }

        foreach (var testCase in sortedMethods.Keys.SelectMany(priority => sortedMethods[priority].OrderBy(
                     testCase => testCase.TestMethod.Method.Name, StringComparer.Ordinal)))
        {
            yield return testCase;
        }
    }

    private static TValue GetOrCreate<TKey, TValue>(SortedDictionary<TKey, TValue> dictionary, TKey key)
        where TKey : struct
        where TValue : new()
    {
        if (!dictionary.TryGetValue(key, out var result))
        {
            result = new TValue();
            dictionary[key] = result;
        }

        return result;
    }
}
