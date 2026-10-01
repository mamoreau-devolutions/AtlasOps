namespace AtlasOps.Features.FinOps.CostAllocationProvisioning;

public sealed record CostAllocationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);