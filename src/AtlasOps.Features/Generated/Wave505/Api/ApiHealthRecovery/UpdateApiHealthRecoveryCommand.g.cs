namespace AtlasOps.Features.Api.ApiHealthRecovery;

public sealed record UpdateApiHealthRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);