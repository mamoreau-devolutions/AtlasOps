namespace AtlasOps.Features.Observability.TraceSpanRecovery;

public sealed record UpdateTraceSpanRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);