namespace AtlasOps.Features.FinOps.CloudInvoiceOptimization;

public sealed record UpdateCloudInvoiceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);