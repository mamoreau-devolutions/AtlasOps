namespace AtlasOps.Features.Incidents.AlertRule;

public sealed record AlertRuleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);