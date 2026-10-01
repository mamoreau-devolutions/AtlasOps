namespace AtlasOps.Features.Governance.ResourceScope;

public sealed record ResourceScopeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);