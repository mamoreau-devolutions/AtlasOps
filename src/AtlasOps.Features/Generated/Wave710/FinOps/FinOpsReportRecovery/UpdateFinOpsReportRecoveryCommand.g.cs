namespace AtlasOps.Features.FinOps.FinOpsReportRecovery;

public sealed record UpdateFinOpsReportRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);