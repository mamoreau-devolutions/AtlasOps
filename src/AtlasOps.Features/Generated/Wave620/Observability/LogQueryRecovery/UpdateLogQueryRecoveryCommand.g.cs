namespace AtlasOps.Features.Observability.LogQueryRecovery;

public sealed record UpdateLogQueryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);