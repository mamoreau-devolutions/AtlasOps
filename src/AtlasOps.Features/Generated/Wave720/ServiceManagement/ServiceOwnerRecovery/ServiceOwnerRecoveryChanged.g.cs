namespace AtlasOps.Features.ServiceManagement.ServiceOwnerRecovery;

public sealed record ServiceOwnerRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);