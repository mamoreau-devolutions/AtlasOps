namespace AtlasOps.Features.Automation.DeploymentFreeze;

using AtlasOps.Features;

public sealed class DeploymentFreezeService(
    IAtlasOpsCapabilityRepository<DeploymentFreezeItem> repository,
    TimeProvider timeProvider)
{
    private readonly DeploymentFreezeValidator validator = new();
    private readonly DeploymentFreezePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DeploymentFreezeChanged>> ExecuteAsync(
        UpdateDeploymentFreezeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DeploymentFreezeChanged>.Invalid(issues);
        }

        DeploymentFreezeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DeploymentFreezeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DeploymentFreezeChanged>.Invalid(
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

        DeploymentFreezeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DeploymentFreezeChanged>.Success(changed);
    }
}