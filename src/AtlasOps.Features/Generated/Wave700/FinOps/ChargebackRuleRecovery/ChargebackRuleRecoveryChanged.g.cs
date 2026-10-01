namespace AtlasOps.Features.FinOps.ChargebackRuleRecovery;

public sealed record ChargebackRuleRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);