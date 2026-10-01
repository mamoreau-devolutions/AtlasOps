namespace AtlasOps.Features.Architecture.ArchitectureReviewProvisioning;

public sealed record UpdateArchitectureReviewProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);