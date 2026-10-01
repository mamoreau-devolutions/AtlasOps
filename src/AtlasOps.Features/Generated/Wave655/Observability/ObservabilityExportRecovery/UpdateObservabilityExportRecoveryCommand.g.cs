namespace AtlasOps.Features.Observability.ObservabilityExportRecovery;

public sealed record UpdateObservabilityExportRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);