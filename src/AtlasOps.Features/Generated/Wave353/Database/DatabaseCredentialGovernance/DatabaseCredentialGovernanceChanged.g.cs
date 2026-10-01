namespace AtlasOps.Features.Database.DatabaseCredentialGovernance;

public sealed record DatabaseCredentialGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);