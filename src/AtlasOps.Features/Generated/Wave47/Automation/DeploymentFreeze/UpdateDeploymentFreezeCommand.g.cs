namespace AtlasOps.Features.Automation.DeploymentFreeze;

public sealed record UpdateDeploymentFreezeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);