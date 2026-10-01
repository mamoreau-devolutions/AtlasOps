namespace AtlasOps.Features.Hardening.RecoveryMode;

public sealed record RecoveryModeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);