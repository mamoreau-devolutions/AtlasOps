namespace AtlasOps.Features.Delivery.ReleaseApprovalGovernance;

public sealed record UpdateReleaseApprovalGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);