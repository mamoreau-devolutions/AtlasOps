namespace AtlasOps.Features.Api.ApiDeploymentRecovery;

public sealed record UpdateApiDeploymentRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);