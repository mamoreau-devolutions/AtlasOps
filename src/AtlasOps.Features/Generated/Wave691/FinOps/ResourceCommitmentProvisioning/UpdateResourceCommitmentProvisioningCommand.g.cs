namespace AtlasOps.Features.FinOps.ResourceCommitmentProvisioning;

public sealed record UpdateResourceCommitmentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);