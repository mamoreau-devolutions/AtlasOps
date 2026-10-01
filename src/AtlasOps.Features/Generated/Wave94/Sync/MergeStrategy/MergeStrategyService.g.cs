namespace AtlasOps.Features.Sync.MergeStrategy;

using AtlasOps.Features;

public sealed class MergeStrategyService(
    IAtlasOpsCapabilityRepository<MergeStrategyItem> repository,
    TimeProvider timeProvider)
{
    private readonly MergeStrategyValidator validator = new();
    private readonly MergeStrategyPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MergeStrategyChanged>> ExecuteAsync(
        UpdateMergeStrategyCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MergeStrategyChanged>.Invalid(issues);
        }

        MergeStrategyItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MergeStrategyItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MergeStrategyChanged>.Invalid(
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

        MergeStrategyChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MergeStrategyChanged>.Success(changed);
    }
}