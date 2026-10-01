namespace AtlasOps.Features.Automation.DeploymentPlan;

using AtlasOps.Features;

public sealed class DeploymentPlanService(
    IAtlasOpsCapabilityRepository<DeploymentPlanItem> repository,
    TimeProvider timeProvider)
{
    private readonly DeploymentPlanValidator validator = new();
    private readonly DeploymentPlanPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DeploymentPlanChanged>> ExecuteAsync(
        UpdateDeploymentPlanCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DeploymentPlanChanged>.Invalid(issues);
        }

        DeploymentPlanItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DeploymentPlanItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DeploymentPlanChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        DeploymentPlanChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DeploymentPlanChanged>.Success(changed);
    }
}