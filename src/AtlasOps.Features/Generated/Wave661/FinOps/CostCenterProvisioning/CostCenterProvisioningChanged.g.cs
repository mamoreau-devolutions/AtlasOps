namespace AtlasOps.Features.FinOps.CostCenterProvisioning;

public sealed record CostCenterProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);