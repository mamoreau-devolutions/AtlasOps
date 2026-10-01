namespace AtlasOps.Features.FinOps.ChargebackRuleMonitoring;

public sealed record ChargebackRuleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);