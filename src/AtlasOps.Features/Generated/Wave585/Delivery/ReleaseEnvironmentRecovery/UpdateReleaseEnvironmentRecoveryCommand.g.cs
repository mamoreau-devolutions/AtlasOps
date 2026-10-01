namespace AtlasOps.Features.Delivery.ReleaseEnvironmentRecovery;

public sealed record UpdateReleaseEnvironmentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);