namespace AtlasOps.Features.FinOps.CostAnomalyProvisioning;

public sealed record CostAnomalyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);