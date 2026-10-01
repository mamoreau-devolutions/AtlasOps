namespace AtlasOps.Features.Compute.ComputeTemplateProvisioning;

public sealed record ComputeTemplateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);