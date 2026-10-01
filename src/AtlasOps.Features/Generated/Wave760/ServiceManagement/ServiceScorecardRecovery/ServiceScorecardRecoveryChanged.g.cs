namespace AtlasOps.Features.ServiceManagement.ServiceScorecardRecovery;

public sealed record ServiceScorecardRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);