namespace AtlasOps.Features.Api.ApiContractProvisioning;

public sealed record UpdateApiContractProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);