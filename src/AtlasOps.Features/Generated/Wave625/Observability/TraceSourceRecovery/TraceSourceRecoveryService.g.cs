namespace AtlasOps.Features.Observability.TraceSourceRecovery;

using AtlasOps.Features;

public sealed class TraceSourceRecoveryService(
    IAtlasOpsCapabilityRepository<TraceSourceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSourceRecoveryValidator validator = new();
    private readonly TraceSourceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSourceRecoveryChanged>> ExecuteAsync(
        UpdateTraceSourceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSourceRecoveryChanged>.Invalid(issues);
        }

        TraceSourceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSourceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSourceRecoveryChanged>.Invalid(
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

        TraceSourceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSourceRecoveryChanged>.Success(changed);
    }
}