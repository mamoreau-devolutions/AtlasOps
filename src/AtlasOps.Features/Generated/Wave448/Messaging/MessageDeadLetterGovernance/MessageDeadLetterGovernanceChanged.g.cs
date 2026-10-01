namespace AtlasOps.Features.Messaging.MessageDeadLetterGovernance;

public sealed record MessageDeadLetterGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);