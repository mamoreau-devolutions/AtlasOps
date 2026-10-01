namespace AtlasOps.Features.ServiceManagement.ServiceScorecardOptimization;

public sealed record ServiceScorecardOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);