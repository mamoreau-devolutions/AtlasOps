namespace AtlasOps.Features.Architecture.ArchitectureReviewRecovery;

public sealed record UpdateArchitectureReviewRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);