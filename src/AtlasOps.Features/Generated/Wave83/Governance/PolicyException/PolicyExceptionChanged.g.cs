namespace AtlasOps.Features.Governance.PolicyException;

public sealed record PolicyExceptionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);