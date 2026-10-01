namespace AtlasOps.Features.Observability.LogQueryRecovery;

using AtlasOps.Features;

public sealed class LogQueryRecoveryService(
    IAtlasOpsCapabilityRepository<LogQueryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogQueryRecoveryValidator validator = new();
    private readonly LogQueryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogQueryRecoveryChanged>> ExecuteAsync(
        UpdateLogQueryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogQueryRecoveryChanged>.Invalid(issues);
        }

        LogQueryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogQueryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogQueryRecoveryChanged>.Invalid(
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

        LogQueryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogQueryRecoveryChanged>.Success(changed);
    }
}