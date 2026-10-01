namespace AtlasOps.Features.Observability.MetricSourceRecovery;

using AtlasOps.Features;

public sealed class MetricSourceRecoveryService(
    IAtlasOpsCapabilityRepository<MetricSourceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricSourceRecoveryValidator validator = new();
    private readonly MetricSourceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricSourceRecoveryChanged>> ExecuteAsync(
        UpdateMetricSourceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricSourceRecoveryChanged>.Invalid(issues);
        }

        MetricSourceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricSourceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricSourceRecoveryChanged>.Invalid(
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

        MetricSourceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricSourceRecoveryChanged>.Success(changed);
    }
}