namespace AtlasOps.Features.Architecture.ArchitectureStandardRecovery;

public sealed record UpdateArchitectureStandardRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);