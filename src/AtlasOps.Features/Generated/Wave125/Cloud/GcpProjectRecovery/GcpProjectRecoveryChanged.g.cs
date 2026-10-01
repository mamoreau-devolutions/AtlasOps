namespace AtlasOps.Features.Cloud.GcpProjectRecovery;

public sealed record GcpProjectRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);