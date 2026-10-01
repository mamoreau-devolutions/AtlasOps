namespace AtlasOps.Features.FinOps.ResourceCommitmentProvisioning;

public sealed record ResourceCommitmentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);