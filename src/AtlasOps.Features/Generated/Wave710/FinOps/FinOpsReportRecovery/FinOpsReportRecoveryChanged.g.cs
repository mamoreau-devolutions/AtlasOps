namespace AtlasOps.Features.FinOps.FinOpsReportRecovery;

public sealed record FinOpsReportRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);