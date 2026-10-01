namespace AtlasOps.Features.Data.DataPipelineProvisioning;

public sealed record DataPipelineProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);