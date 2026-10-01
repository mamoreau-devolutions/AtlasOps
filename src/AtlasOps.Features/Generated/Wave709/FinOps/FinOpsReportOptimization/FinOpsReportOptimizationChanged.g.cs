namespace AtlasOps.Features.FinOps.FinOpsReportOptimization;

public sealed record FinOpsReportOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);