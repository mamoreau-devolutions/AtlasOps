namespace AtlasOps.Features.Observability.TraceSpanGovernance;

public sealed record UpdateTraceSpanGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);