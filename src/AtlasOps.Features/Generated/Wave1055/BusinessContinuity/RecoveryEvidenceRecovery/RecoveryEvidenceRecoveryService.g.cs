namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceRecovery;

using AtlasOps.Features;

public sealed class RecoveryEvidenceRecoveryService(
    IAtlasOpsCapabilityRepository<RecoveryEvidenceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryEvidenceRecoveryValidator validator = new();
    private readonly RecoveryEvidenceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryEvidenceRecoveryChanged>> ExecuteAsync(
        UpdateRecoveryEvidenceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryEvidenceRecoveryChanged>.Invalid(issues);
        }

        RecoveryEvidenceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryEvidenceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryEvidenceRecoveryChanged>.Invalid(
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

        RecoveryEvidenceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryEvidenceRecoveryChanged>.Success(changed);
    }
}