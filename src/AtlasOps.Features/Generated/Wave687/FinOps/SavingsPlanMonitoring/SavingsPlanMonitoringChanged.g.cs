namespace AtlasOps.Features.FinOps.SavingsPlanMonitoring;

public sealed record SavingsPlanMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);