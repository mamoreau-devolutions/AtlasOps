namespace AtlasOps.Features.Api.ApiAnalyticsRecovery;

public sealed record UpdateApiAnalyticsRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);