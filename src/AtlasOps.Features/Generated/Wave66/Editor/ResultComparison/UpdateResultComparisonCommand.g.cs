namespace AtlasOps.Features.Editor.ResultComparison;

public sealed record UpdateResultComparisonCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);