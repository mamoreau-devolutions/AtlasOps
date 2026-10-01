namespace AtlasOps.Features.Cloud.AzureSubscriptionMonitoring;

public sealed record AzureSubscriptionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);