namespace AtlasOps.Features.Analytics.HealthWidget;

public sealed record HealthWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);