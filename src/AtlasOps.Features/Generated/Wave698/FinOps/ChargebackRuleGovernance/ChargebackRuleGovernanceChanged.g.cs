namespace AtlasOps.Features.FinOps.ChargebackRuleGovernance;

public sealed record ChargebackRuleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);