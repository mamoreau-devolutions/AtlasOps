namespace AtlasOps.Features.ServiceManagement.ServiceRequestRecovery;

public sealed record ServiceRequestRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);