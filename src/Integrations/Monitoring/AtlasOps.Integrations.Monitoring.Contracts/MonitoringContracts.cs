namespace AtlasOps.Integrations.Monitoring.Contracts;

public sealed record MetricSample(
    string Source,
    string Name,
    DateTimeOffset Timestamp,
    double Value,
    IReadOnlyDictionary<string, string> Dimensions);

public sealed record AlertSignal(
    string Source,
    string AlertId,
    string ResourceId,
    string Severity,
    string Summary,
    DateTimeOffset StartedAt,
    DateTimeOffset? ResolvedAt,
    IReadOnlyDictionary<string, string> Dimensions);

public sealed record AlertCorrelationGroup(
    string CorrelationKey,
    string ResourceId,
    string HighestSeverity,
    DateTimeOffset StartedAt,
    DateTimeOffset? ResolvedAt,
    IReadOnlyList<AlertSignal> Alerts);

public sealed record MetricWindow(
    DateTimeOffset Start,
    DateTimeOffset End,
    double Minimum,
    double Maximum,
    double Average,
    int SampleCount);
