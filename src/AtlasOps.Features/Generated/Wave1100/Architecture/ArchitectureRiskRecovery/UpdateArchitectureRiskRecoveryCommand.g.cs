namespace AtlasOps.Features.Architecture.ArchitectureRiskRecovery;

public sealed record UpdateArchitectureRiskRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);