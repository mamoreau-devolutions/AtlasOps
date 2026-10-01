namespace AtlasOps.Features.Observability.TraceSourceOptimization;

using AtlasOps.Features;

public sealed class TraceSourceOptimizationService(
    IAtlasOpsCapabilityRepository<TraceSourceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSourceOptimizationValidator validator = new();
    private readonly TraceSourceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSourceOptimizationChanged>> ExecuteAsync(
        UpdateTraceSourceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSourceOptimizationChanged>.Invalid(issues);
        }

        TraceSourceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSourceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSourceOptimizationChanged>.Invalid(
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

        TraceSourceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSourceOptimizationChanged>.Success(changed);
    }
}