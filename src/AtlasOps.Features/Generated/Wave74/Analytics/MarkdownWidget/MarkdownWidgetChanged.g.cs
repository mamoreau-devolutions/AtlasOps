namespace AtlasOps.Features.Analytics.MarkdownWidget;

public sealed record MarkdownWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);