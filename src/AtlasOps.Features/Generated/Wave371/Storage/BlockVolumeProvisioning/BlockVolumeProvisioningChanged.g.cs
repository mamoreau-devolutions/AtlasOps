namespace AtlasOps.Features.Storage.BlockVolumeProvisioning;

public sealed record BlockVolumeProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);