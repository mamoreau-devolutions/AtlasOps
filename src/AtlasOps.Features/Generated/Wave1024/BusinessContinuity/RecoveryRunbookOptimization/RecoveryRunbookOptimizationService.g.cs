namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookOptimization;

using AtlasOps.Features;

public sealed class RecoveryRunbookOptimizationService(
    IAtlasOpsCapabilityRepository<RecoveryRunbookOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryRunbookOptimizationValidator validator = new();
    private readonly RecoveryRunbookOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryRunbookOptimizationChanged>> ExecuteAsync(
        UpdateRecoveryRunbookOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryRunbookOptimizationChanged>.Invalid(issues);
        }

        RecoveryRunbookOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryRunbookOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryRunbookOptimizationChanged>.Invalid(
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

        RecoveryRunbookOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryRunbookOptimizationChanged>.Success(changed);
    }
}