namespace AtlasOps.Features.Governance.AccessRequest;

public sealed record AccessRequestChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);