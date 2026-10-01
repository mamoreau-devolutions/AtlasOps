namespace AtlasOps.Features.Api.ApiContractMonitoring;

public sealed record ApiContractMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);