namespace AtlasOps.Features.Desktop.DesktopUpdateRecovery;

public sealed record DesktopUpdateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);