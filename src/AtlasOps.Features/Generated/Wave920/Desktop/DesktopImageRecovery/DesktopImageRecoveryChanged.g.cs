namespace AtlasOps.Features.Desktop.DesktopImageRecovery;

public sealed record DesktopImageRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);