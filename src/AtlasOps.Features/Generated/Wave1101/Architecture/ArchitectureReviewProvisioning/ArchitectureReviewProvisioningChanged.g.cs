namespace AtlasOps.Features.Architecture.ArchitectureReviewProvisioning;

public sealed record ArchitectureReviewProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);