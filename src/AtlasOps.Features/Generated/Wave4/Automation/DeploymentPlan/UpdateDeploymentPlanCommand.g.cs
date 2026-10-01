namespace AtlasOps.Features.Automation.DeploymentPlan;

public sealed record UpdateDeploymentPlanCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);