namespace AtlasOps.Features.Storage.FileShareMonitoring;

public sealed record FileShareMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);