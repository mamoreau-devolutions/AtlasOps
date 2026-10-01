namespace AtlasOps.Features.Observability.TraceSpanRecovery;

using AtlasOps.Features;

public sealed class TraceSpanRecoveryService(
    IAtlasOpsCapabilityRepository<TraceSpanRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly TraceSpanRecoveryValidator validator = new();
    private readonly TraceSpanRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TraceSpanRecoveryChanged>> ExecuteAsync(
        UpdateTraceSpanRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TraceSpanRecoveryChanged>.Invalid(issues);
        }

        TraceSpanRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TraceSpanRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TraceSpanRecoveryChanged>.Invalid(
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

        TraceSpanRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TraceSpanRecoveryChanged>.Success(changed);
    }
}