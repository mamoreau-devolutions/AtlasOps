namespace AtlasOps.Features.ServiceManagement.ProblemRecordOptimization;

public sealed record ProblemRecordOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);