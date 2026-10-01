namespace AtlasOps.Features.Automation.DeploymentStage;

public sealed record UpdateDeploymentStageCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);