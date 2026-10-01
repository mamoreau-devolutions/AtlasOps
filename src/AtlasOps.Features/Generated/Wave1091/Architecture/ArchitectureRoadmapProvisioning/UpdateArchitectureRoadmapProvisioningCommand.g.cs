namespace AtlasOps.Features.Architecture.ArchitectureRoadmapProvisioning;

public sealed record UpdateArchitectureRoadmapProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);