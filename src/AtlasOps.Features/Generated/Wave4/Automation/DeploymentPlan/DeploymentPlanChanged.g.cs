namespace AtlasOps.Features.Automation.DeploymentPlan;

public sealed record DeploymentPlanChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);