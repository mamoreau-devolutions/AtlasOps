namespace AtlasOps.Features.Observability.LogSourceRecovery;

using AtlasOps.Features;

public sealed class LogSourceRecoveryService(
    IAtlasOpsCapabilityRepository<LogSourceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogSourceRecoveryValidator validator = new();
    private readonly LogSourceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogSourceRecoveryChanged>> ExecuteAsync(
        UpdateLogSourceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogSourceRecoveryChanged>.Invalid(issues);
        }

        LogSourceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogSourceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogSourceRecoveryChanged>.Invalid(
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

        LogSourceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogSourceRecoveryChanged>.Success(changed);
    }
}