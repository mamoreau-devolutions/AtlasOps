namespace AtlasOps.Features.Api.ApiAnalyticsProvisioning;

public sealed record UpdateApiAnalyticsProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);