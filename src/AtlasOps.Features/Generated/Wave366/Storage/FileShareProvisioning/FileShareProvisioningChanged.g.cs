namespace AtlasOps.Features.Storage.FileShareProvisioning;

public sealed record FileShareProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);