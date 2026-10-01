namespace AtlasOps.Features.Hardening.UpgradeChannel;

public sealed record UpgradeChannelChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);