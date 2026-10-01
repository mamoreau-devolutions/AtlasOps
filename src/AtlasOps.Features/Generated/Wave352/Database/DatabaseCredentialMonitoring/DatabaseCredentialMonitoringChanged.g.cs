namespace AtlasOps.Features.Database.DatabaseCredentialMonitoring;

public sealed record DatabaseCredentialMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);