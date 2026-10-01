namespace AtlasOps.Features.Api.ApiAnalyticsGovernance;

public sealed record UpdateApiAnalyticsGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);