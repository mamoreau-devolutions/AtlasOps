namespace AtlasOps.Features.Observability.TraceSourceRecovery;

public sealed record UpdateTraceSourceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);