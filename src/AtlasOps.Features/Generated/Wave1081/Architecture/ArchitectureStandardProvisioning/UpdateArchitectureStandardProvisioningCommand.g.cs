namespace AtlasOps.Features.Architecture.ArchitectureStandardProvisioning;

public sealed record UpdateArchitectureStandardProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);