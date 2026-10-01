namespace AtlasOps.Features.Analytics.MetricFormula;

public sealed record MetricFormulaChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);