namespace AtlasOps.Features.FinOps.ChargebackRuleGovernance;

public sealed record UpdateChargebackRuleGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);