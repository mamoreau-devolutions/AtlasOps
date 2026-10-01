namespace AtlasOps.Features.Storage.BlockVolumeRecovery;

public sealed record BlockVolumeRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);