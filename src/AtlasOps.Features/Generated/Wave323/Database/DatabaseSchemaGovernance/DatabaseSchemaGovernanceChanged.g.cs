namespace AtlasOps.Features.Database.DatabaseSchemaGovernance;

public sealed record DatabaseSchemaGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);