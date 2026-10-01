namespace AtlasOps.Features.FinOps.ChargebackRuleRecovery;

public sealed record UpdateChargebackRuleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);