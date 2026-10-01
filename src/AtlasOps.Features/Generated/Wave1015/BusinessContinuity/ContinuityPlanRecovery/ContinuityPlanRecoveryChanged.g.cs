namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanRecovery;

public sealed record ContinuityPlanRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);