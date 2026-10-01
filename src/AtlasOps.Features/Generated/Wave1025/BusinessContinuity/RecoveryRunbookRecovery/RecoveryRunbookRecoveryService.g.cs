namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookRecovery;

using AtlasOps.Features;

public sealed class RecoveryRunbookRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryRunbookRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryRunbookRecoveryValidator validator = new();
    private readonly RecoveryRunbookRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryRunbookRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryRunbookRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryRunbookRecoveryChanged>.Invalid(issues);
        }

        RecoveryRunbookRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryRunbookRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryRunbookRecoveryChanged>.Invalid(
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

        RecoveryRunbookRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryRunbookRecoveryChanged>.Success(changed);
    }
}