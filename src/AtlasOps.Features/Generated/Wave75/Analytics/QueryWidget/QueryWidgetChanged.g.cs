namespace AtlasOps.Features.Analytics.QueryWidget;

public sealed record QueryWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);