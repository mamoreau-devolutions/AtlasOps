namespace AtlasOps.Features.ServiceManagement.ServiceRequestGovernance;

public sealed record UpdateServiceRequestGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);