namespace AtlasOps.Features.Observability.LogSourceOptimization;

using AtlasOps.Features;

public sealed class LogSourceOptimizationService(
    IAtlasOpsCapabilityRepository<LogSourceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogSourceOptimizationValidator validator = new();
    private readonly LogSourceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogSourceOptimizationChanged>> ExecuteAsync(
        UpdateLogSourceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogSourceOptimizationChanged>.Invalid(issues);
        }

        LogSourceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogSourceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogSourceOptimizationChanged>.Invalid(
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

        LogSourceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogSourceOptimizationChanged>.Success(changed);
    }
}