namespace AtlasOps.Features.Analytics.DashboardParameter;

public sealed record DashboardParameterChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);