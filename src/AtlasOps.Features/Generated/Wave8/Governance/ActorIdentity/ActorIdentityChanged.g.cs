namespace AtlasOps.Features.Governance.ActorIdentity;

public sealed record ActorIdentityChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);