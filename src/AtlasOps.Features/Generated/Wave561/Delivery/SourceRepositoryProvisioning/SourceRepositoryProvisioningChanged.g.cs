namespace AtlasOps.Features.Delivery.SourceRepositoryProvisioning;

public sealed record SourceRepositoryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);