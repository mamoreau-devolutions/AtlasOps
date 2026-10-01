namespace AtlasOps.Features.Api.ApiQuotaProvisioning;

public sealed record UpdateApiQuotaProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);