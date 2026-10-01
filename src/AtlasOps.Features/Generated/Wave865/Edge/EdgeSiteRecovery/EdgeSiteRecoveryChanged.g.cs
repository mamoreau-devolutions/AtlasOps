namespace AtlasOps.Features.Edge.EdgeSiteRecovery;

public sealed record EdgeSiteRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);