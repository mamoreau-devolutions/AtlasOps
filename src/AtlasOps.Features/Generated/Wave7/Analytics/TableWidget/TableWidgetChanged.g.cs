namespace AtlasOps.Features.Analytics.TableWidget;

public sealed record TableWidgetChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);