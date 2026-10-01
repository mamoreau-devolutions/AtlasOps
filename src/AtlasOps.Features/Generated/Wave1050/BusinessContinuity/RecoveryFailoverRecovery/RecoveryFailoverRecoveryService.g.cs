namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverRecovery;

using AtlasOps.Features;

public sealed class RecoveryFailoverRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryFailoverRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryFailoverRecoveryValidator validator = new();
    private readonly RecoveryFailoverRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryFailoverRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged>.Invalid(issues);
        }

        RecoveryFailoverRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryFailoverRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged>.Invalid(
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

        RecoveryFailoverRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged>.Success(changed);
    }
}