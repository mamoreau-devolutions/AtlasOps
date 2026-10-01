namespace AtlasOps.Features.Governance.RetentionPolicy;

public sealed record RetentionPolicyChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);