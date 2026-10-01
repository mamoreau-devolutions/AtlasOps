namespace AtlasOps.Features.ServiceManagement.ProblemRecordRecovery;

using AtlasOps.Features;

public sealed class ProblemRecordRecoveryService(
    IAtlasOpsCapabilityRepository<ProblemRecordRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProblemRecordRecoveryValidator validator = new();
    private readonly ProblemRecordRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProblemRecordRecoveryChanged>> ExecuteAsync(
        UpdateProblemRecordRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProblemRecordRecoveryChanged>.Invalid(issues);
        }

        ProblemRecordRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProblemRecordRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProblemRecordRecoveryChanged>.Invalid(
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

        ProblemRecordRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProblemRecordRecoveryChanged>.Success(changed);
    }
}