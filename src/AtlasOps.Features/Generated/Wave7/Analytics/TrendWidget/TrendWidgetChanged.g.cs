namespace AtlasOps.Features.Analytics.TrendWidget;

public sealed record TrendWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);