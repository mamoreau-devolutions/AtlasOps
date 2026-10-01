namespace AtlasOps.Features.ServiceManagement.ServiceDependencyGovernance;

public sealed record UpdateServiceDependencyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);