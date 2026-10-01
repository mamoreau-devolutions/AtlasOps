namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowGovernance;

public sealed record MaintenanceWindowGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);