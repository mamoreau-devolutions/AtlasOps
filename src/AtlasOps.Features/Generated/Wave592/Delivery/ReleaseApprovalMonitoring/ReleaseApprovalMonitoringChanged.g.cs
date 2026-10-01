namespace AtlasOps.Features.Delivery.ReleaseApprovalMonitoring;

public sealed record ReleaseApprovalMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);