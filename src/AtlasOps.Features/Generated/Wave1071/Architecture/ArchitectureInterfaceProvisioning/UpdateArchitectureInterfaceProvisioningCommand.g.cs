namespace AtlasOps.Features.Architecture.ArchitectureInterfaceProvisioning;

public sealed record UpdateArchitectureInterfaceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);