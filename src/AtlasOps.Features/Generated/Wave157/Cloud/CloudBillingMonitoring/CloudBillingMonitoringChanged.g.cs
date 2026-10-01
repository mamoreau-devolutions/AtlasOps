namespace AtlasOps.Features.Cloud.CloudBillingMonitoring;

public sealed record CloudBillingMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);