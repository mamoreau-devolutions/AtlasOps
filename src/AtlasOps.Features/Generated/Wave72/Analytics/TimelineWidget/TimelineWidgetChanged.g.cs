namespace AtlasOps.Features.Analytics.TimelineWidget;

public sealed record TimelineWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);