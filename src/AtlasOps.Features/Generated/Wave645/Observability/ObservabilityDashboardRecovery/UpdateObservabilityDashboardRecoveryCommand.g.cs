namespace AtlasOps.Features.Observability.ObservabilityDashboardRecovery;

public sealed record UpdateObservabilityDashboardRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);