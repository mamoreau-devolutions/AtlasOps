namespace AtlasOps.Features.Analytics.NumberWidget;

public sealed record NumberWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);