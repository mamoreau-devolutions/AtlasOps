namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverOptimization;

using AtlasOps.Features;

public sealed class RecoveryFailoverOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryFailoverOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryFailoverOptimizationValidator validator = new();
    private readonly RecoveryFailoverOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryFailoverOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryFailoverOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryFailoverOptimizationChanged>.Invalid(issues);
        }

        RecoveryFailoverOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryFailoverOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryFailoverOptimizationChanged>.Invalid(
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

        RecoveryFailoverOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryFailoverOptimizationChanged>.Success(changed);
    }
}