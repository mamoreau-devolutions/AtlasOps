namespace AtlasOps.Features.Edge.EdgeDeviceRecovery;

public sealed record EdgeDeviceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);