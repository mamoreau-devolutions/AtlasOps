namespace AtlasOps.Features.Api.ApiClientRecovery;

public sealed record UpdateApiClientRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);