namespace AtlasOps.Features.Architecture.ArchitectureDecisionRecovery;

public sealed record UpdateArchitectureDecisionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);