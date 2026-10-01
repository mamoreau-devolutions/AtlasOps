namespace AtlasOps.Features.ServiceManagement.ServiceOwnerGovernance;

public sealed record UpdateServiceOwnerGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);