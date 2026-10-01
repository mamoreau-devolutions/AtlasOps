namespace AtlasOps.Features.Cloud.AwsAccountMonitoring;

public sealed record AwsAccountMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);