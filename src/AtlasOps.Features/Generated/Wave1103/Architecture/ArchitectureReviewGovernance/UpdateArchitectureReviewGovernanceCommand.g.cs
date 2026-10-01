namespace AtlasOps.Features.Architecture.ArchitectureReviewGovernance;

public sealed record UpdateArchitectureReviewGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);