namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveRecovery;

using AtlasOps.Features;

public sealed class RecoveryObjectiveRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryObjectiveRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryObjectiveRecoveryValidator validator = new();
    private readonly RecoveryObjectiveRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryObjectiveRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryObjectiveRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryObjectiveRecoveryChanged>.Invalid(issues);
        }

        RecoveryObjectiveRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryObjectiveRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryObjectiveRecoveryChanged>.Invalid(
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

        RecoveryObjectiveRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryObjectiveRecoveryChanged>.Success(changed);
    }
}