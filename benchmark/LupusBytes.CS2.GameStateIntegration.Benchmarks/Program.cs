using BenchmarkDotNet.Running;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

/// <summary>
/// Entry point marker, required to reference the assembly from the top level statements above.
/// </summary>
public partial class Program;
