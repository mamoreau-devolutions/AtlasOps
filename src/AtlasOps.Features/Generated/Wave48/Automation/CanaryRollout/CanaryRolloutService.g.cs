namespace AtlasOps.Features.Automation.CanaryRollout;

using AtlasOps.Features;

public sealed class CanaryRolloutService(
    IAtlasOpsCapabilityRepository<CanaryRolloutItem> repository,
    TimeProvider timeProvider)
{
    private readonly CanaryRolloutValidator validator = new();
    private readonly CanaryRolloutPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CanaryRolloutChanged>> ExecuteAsync(
        UpdateCanaryRolloutCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CanaryRolloutChanged>.Invalid(issues);
        }

        CanaryRolloutItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CanaryRolloutItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CanaryRolloutChanged>.Invalid(
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

        CanaryRolloutChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CanaryRolloutChanged>.Success(changed);
    }
}