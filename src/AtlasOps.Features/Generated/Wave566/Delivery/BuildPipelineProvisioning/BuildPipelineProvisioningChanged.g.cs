namespace AtlasOps.Features.Delivery.BuildPipelineProvisioning;

public sealed record BuildPipelineProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);