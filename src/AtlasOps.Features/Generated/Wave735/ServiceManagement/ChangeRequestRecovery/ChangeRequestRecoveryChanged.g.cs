namespace AtlasOps.Features.ServiceManagement.ChangeRequestRecovery;

public sealed record ChangeRequestRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);