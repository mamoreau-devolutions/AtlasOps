namespace AtlasOps.Features.FinOps.FinOpsReportProvisioning;

public sealed record UpdateFinOpsReportProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);