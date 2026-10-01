namespace AtlasOps.Features.Compute.ComputeTemplateProvisioning;

public sealed record UpdateComputeTemplateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);