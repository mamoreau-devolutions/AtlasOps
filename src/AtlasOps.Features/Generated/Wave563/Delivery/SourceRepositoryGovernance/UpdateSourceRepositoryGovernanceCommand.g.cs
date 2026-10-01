namespace AtlasOps.Features.Delivery.SourceRepositoryGovernance;

public sealed record UpdateSourceRepositoryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);