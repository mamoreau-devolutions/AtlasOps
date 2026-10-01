namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyRecovery;

using AtlasOps.Features;

public sealed class RecoveryDependencyRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryDependencyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryDependencyRecoveryValidator validator = new();
    private readonly RecoveryDependencyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryDependencyRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryDependencyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryDependencyRecoveryChanged>.Invalid(issues);
        }

        RecoveryDependencyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryDependencyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryDependencyRecoveryChanged>.Invalid(
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

        RecoveryDependencyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryDependencyRecoveryChanged>.Success(changed);
    }
}