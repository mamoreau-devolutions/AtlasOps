namespace AtlasOps.Features.Inventory.TopologyProjection;

public sealed record TopologyProjectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);