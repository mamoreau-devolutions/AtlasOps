namespace AtlasOps.Features.Architecture.ArchitectureComponentProvisioning;

public sealed record UpdateArchitectureComponentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);