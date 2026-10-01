namespace AtlasOps.Features.Automation.DeploymentStage;

public sealed record DeploymentStageChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);