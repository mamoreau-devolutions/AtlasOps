namespace AtlasOps.Features.Analytics.MetricFormula;

public sealed record UpdateMetricFormulaCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);