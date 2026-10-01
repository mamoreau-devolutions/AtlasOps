namespace AtlasOps.Features.Desktop.DesktopPolicyMonitoring;

public sealed record DesktopPolicyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);