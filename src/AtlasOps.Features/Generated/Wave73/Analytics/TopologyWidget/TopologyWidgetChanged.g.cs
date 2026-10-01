namespace AtlasOps.Features.Analytics.TopologyWidget;

public sealed record TopologyWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);