namespace AtlasOps.Features.Security.SecurityPatchMonitoring;

public sealed record SecurityPatchMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);