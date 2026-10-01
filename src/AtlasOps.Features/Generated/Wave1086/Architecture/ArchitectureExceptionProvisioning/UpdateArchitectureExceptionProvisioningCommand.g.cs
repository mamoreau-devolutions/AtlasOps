namespace AtlasOps.Features.Architecture.ArchitectureExceptionProvisioning;

public sealed record UpdateArchitectureExceptionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);