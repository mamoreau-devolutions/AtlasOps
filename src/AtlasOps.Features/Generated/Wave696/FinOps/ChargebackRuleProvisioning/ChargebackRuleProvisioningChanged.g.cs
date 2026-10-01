namespace AtlasOps.Features.FinOps.ChargebackRuleProvisioning;

public sealed record ChargebackRuleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);