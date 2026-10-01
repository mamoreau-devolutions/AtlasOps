namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowGovernance;

public sealed record UpdateMaintenanceWindowGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);