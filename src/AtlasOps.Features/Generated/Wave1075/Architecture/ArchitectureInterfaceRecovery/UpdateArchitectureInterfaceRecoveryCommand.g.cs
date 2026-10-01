namespace AtlasOps.Features.Architecture.ArchitectureInterfaceRecovery;

public sealed record UpdateArchitectureInterfaceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);