namespace AtlasOps.Features.Automation.BlueGreenRollout;

using AtlasOps.Features;

public sealed class BlueGreenRolloutService(
    IAtlasOpsCapabilityRepository<BlueGreenRolloutItem> repository,
    TimeProvider timeProvider)
{
    private readonly BlueGreenRolloutValidator validator = new();
    private readonly BlueGreenRolloutPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BlueGreenRolloutChanged>> ExecuteAsync(
        UpdateBlueGreenRolloutCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BlueGreenRolloutChanged>.Invalid(issues);
        }

        BlueGreenRolloutItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BlueGreenRolloutItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BlueGreenRolloutChanged>.Invalid(
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

        BlueGreenRolloutChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BlueGreenRolloutChanged>.Success(changed);
    }
}