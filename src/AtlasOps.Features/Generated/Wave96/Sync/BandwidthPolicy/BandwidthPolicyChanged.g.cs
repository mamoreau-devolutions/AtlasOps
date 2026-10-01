namespace AtlasOps.Features.Sync.BandwidthPolicy;

public sealed record BandwidthPolicyChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);