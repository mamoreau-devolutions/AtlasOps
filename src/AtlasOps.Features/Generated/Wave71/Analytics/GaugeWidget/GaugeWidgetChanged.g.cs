namespace AtlasOps.Features.Analytics.GaugeWidget;

public sealed record GaugeWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);