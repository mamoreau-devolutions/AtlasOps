namespace AtlasOps.Features.Desktop.DesktopSessionRecovery;

public sealed record DesktopSessionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);