namespace AtlasOps.Features.FinOps.FinOpsReportGovernance;

public sealed record FinOpsReportGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);