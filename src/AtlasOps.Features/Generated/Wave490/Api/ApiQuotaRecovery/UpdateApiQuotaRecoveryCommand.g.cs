namespace AtlasOps.Features.Api.ApiQuotaRecovery;

public sealed record UpdateApiQuotaRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);