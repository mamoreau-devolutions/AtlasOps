namespace AtlasOps.Features.Desktop.DesktopPolicyRecovery;

public sealed record DesktopPolicyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);