namespace AtlasOps.Features.Architecture.ArchitectureDecisionProvisioning;

public sealed record UpdateArchitectureDecisionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);