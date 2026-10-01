namespace AtlasOps.Features.BusinessContinuity.RecoverySiteRecovery;

using AtlasOps.Features;

public sealed class RecoverySiteRecoveryService(
    IAtlasOpsCapabilityRepository<RecoverySiteRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoverySiteRecoveryValidator validator = new();
    private readonly RecoverySiteRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoverySiteRecoveryChanged>> ExecuteAsync(
        UpdateRecoverySiteRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoverySiteRecoveryChanged>.Invalid(issues);
        }

        RecoverySiteRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoverySiteRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoverySiteRecoveryChanged>.Invalid(
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

        RecoverySiteRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoverySiteRecoveryChanged>.Success(changed);
    }
}