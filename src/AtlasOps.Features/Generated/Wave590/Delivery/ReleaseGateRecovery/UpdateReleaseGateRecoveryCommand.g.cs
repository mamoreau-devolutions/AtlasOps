namespace AtlasOps.Features.Delivery.ReleaseGateRecovery;

public sealed record UpdateReleaseGateRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);