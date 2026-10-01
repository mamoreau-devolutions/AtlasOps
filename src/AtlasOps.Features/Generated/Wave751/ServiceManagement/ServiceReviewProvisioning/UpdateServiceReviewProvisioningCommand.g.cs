namespace AtlasOps.Features.ServiceManagement.ServiceReviewProvisioning;

public sealed record UpdateServiceReviewProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);