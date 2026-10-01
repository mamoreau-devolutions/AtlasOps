namespace AtlasOps.Connectors.Runtime;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NodaTime;

public sealed record RuntimePackageDescriptor(string Id, Type PrimaryType, string Capability);

public static class RuntimePackageCatalog
{
    public static IReadOnlyList<RuntimePackageDescriptor> Packages { get; } =
    [
        new("memory-cache", typeof(MemoryCache), "Bounded connector snapshot caching"),
        new("dependency-injection", typeof(ServiceCollection), "Explicit connector composition"),
        new("options", typeof(OptionsFactory<>), "Validated connector configuration"),
        new("polly-http", typeof(Polly.Policy), "Transient HTTP resilience"),
        new("noda-time", typeof(Instant), "Portable timestamps and scheduling"),
        new("rules-engine", typeof(RulesEngine.Models.Rule), "Connector eligibility rules"),
    ];
}
