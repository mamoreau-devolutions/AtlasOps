namespace AtlasOps.Features.Delivery.BuildArtifactProvisioning;

public sealed record UpdateBuildArtifactProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);