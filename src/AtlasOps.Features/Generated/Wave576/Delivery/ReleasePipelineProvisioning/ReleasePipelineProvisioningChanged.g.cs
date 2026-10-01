namespace AtlasOps.Features.Delivery.ReleasePipelineProvisioning;

public sealed record ReleasePipelineProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);