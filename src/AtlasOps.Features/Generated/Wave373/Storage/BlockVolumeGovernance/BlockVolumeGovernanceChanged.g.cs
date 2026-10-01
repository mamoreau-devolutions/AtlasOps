namespace AtlasOps.Features.Storage.BlockVolumeGovernance;

public sealed record BlockVolumeGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);