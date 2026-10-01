namespace AtlasOps.Features.Api.ApiQuotaGovernance;

public sealed record UpdateApiQuotaGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);