namespace AtlasOps.Features.Hardening.SupportBundle;

public sealed record SupportBundleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);