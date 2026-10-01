namespace AtlasOps.Features.Delivery.ReleaseRollbackRecovery;

public sealed record UpdateReleaseRollbackRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);