namespace AtlasOps.Features.ServiceManagement.ServiceReviewGovernance;

public sealed record UpdateServiceReviewGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);