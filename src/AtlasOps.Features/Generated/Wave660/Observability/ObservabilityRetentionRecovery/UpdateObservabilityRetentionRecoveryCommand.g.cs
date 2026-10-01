namespace AtlasOps.Features.Observability.ObservabilityRetentionRecovery;

public sealed record UpdateObservabilityRetentionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);