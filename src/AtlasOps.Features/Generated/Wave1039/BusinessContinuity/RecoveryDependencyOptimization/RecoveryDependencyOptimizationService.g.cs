namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyOptimization;

using AtlasOps.Features;

public sealed class RecoveryDependencyOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryDependencyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryDependencyOptimizationValidator validator = new();
    private readonly RecoveryDependencyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryDependencyOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryDependencyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryDependencyOptimizationChanged>.Invalid(issues);
        }

        RecoveryDependencyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryDependencyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryDependencyOptimizationChanged>.Invalid(
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

        RecoveryDependencyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryDependencyOptimizationChanged>.Success(changed);
    }
}