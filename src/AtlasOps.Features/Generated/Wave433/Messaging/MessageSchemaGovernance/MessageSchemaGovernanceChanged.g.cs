namespace AtlasOps.Features.Messaging.MessageSchemaGovernance;

public sealed record MessageSchemaGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);