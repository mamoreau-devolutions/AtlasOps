namespace AtlasOps.Features.Compute.ComputeScaleSetProvisioning;

public sealed record ComputeScaleSetProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);