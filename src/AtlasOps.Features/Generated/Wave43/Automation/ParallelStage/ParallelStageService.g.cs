namespace AtlasOps.Features.Automation.ParallelStage;

using AtlasOps.Features;

public sealed class ParallelStageService(
    IAtlasOpsCapabilityRepository<ParallelStageItem> repository,
    TimeProvider timeProvider)
{
    private readonly ParallelStageValidator validator = new();
    private readonly ParallelStagePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ParallelStageChanged>> ExecuteAsync(
        UpdateParallelStageCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ParallelStageChanged>.Invalid(issues);
        }

        ParallelStageItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ParallelStageItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ParallelStageChanged>.Invalid(
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

        ParallelStageChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ParallelStageChanged>.Success(changed);
    }
}