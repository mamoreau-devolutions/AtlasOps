namespace AtlasOps.Features.FinOps.ChargebackRuleProvisioning;

public sealed record UpdateChargebackRuleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);