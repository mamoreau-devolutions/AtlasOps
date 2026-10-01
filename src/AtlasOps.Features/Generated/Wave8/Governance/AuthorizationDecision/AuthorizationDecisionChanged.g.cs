namespace AtlasOps.Features.Governance.AuthorizationDecision;

public sealed record AuthorizationDecisionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);