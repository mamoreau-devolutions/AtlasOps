namespace AtlasOps.Features.Analytics.DashboardLayout;

public sealed record DashboardLayoutChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);