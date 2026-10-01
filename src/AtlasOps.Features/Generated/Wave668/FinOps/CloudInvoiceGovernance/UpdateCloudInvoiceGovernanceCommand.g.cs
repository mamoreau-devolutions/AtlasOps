namespace AtlasOps.Features.FinOps.CloudInvoiceGovernance;

public sealed record UpdateCloudInvoiceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);