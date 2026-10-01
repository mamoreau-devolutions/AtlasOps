namespace AtlasOps.Features.Identity.IdentityRoleMonitoring;

public sealed record IdentityRoleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);