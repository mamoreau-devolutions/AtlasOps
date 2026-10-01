namespace AtlasOps.Features.BusinessContinuity.RecoverySiteOptimization;

using AtlasOps.Features;

public sealed class RecoverySiteOptimizationService(
    IAtlasOpsCapabilityRepository<RecoverySiteOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoverySiteOptimizationValidator validator = new();
    private readonly RecoverySiteOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoverySiteOptimizationChanged>> ExecuteAsync(
        UpdateRecoverySiteOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoverySiteOptimizationChanged>.Invalid(issues);
        }

        RecoverySiteOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoverySiteOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoverySiteOptimizationChanged>.Invalid(
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

        RecoverySiteOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoverySiteOptimizationChanged>.Success(changed);
    }
}