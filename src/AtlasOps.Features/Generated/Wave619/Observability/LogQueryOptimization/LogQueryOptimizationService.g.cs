namespace AtlasOps.Features.Observability.LogQueryOptimization;

using AtlasOps.Features;

public sealed class LogQueryOptimizationService(
    IAtlasOpsCapabilityRepository<LogQueryOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogQueryOptimizationValidator validator = new();
    private readonly LogQueryOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogQueryOptimizationChanged>> ExecuteAsync(
        UpdateLogQueryOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogQueryOptimizationChanged>.Invalid(issues);
        }

        LogQueryOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogQueryOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogQueryOptimizationChanged>.Invalid(
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

        LogQueryOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogQueryOptimizationChanged>.Success(changed);
    }
}