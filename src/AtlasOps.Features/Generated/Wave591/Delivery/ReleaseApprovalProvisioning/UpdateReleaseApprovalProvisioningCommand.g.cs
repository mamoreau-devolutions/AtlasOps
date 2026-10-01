namespace AtlasOps.Features.Delivery.ReleaseApprovalProvisioning;

public sealed record UpdateReleaseApprovalProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);