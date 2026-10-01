namespace AtlasOps.Features.FinOps.CloudInvoiceProvisioning;

public sealed record UpdateCloudInvoiceProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);