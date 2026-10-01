namespace AtlasOps.Features.FinOps.CloudInvoiceRecovery;

public sealed record UpdateCloudInvoiceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);