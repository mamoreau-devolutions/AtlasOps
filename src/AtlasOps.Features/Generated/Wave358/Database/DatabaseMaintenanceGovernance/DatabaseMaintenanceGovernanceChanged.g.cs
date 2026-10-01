namespace AtlasOps.Features.Database.DatabaseMaintenanceGovernance;

public sealed record DatabaseMaintenanceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);