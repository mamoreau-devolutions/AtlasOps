namespace AtlasOps.Features.Architecture.ArchitectureExceptionRecovery;

public sealed record UpdateArchitectureExceptionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);