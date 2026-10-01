namespace AtlasOps.Features.Messaging.MessageRetentionGovernance;

public sealed record MessageRetentionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);