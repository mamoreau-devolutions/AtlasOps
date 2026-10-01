namespace AtlasOps.Features.Architecture.ArchitectureComponentRecovery;

public sealed record UpdateArchitectureComponentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);