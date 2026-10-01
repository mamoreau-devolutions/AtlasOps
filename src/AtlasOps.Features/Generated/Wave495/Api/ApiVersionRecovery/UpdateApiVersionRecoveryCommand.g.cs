namespace AtlasOps.Features.Api.ApiVersionRecovery;

public sealed record UpdateApiVersionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);