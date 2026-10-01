namespace AtlasOps.Features.Delivery.ReleaseApprovalRecovery;

public sealed record UpdateReleaseApprovalRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);