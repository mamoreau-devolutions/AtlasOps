namespace AtlasOps.Features.Cloud.CloudFunctionMonitoring;

public sealed record CloudFunctionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);