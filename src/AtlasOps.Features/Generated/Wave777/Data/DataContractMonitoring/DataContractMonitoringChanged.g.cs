namespace AtlasOps.Features.Data.DataContractMonitoring;

public sealed record DataContractMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);