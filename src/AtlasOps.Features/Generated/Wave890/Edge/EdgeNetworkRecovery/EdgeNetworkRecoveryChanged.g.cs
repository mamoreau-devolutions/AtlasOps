namespace AtlasOps.Features.Edge.EdgeNetworkRecovery;

public sealed record EdgeNetworkRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);