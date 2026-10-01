namespace AtlasOps.Features.Architecture.ArchitectureDependencyRecovery;

public sealed record UpdateArchitectureDependencyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);