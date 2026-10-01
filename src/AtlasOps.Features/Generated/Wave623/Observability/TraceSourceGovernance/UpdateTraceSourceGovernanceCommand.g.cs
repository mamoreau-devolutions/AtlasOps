namespace AtlasOps.Features.Observability.TraceSourceGovernance;

public sealed record UpdateTraceSourceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);