namespace AtlasOps.Features.Observability.ObservabilitySloRecovery;

public sealed record UpdateObservabilitySloRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);