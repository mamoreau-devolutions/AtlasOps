namespace AtlasOps.Features.Automation.DeploymentFreeze;

public sealed record DeploymentFreezeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);