namespace AtlasOps.Features.Architecture.ArchitectureRiskProvisioning;

public sealed record UpdateArchitectureRiskProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);