namespace AtlasOps.Features.Observability.MetricAlertRecovery;

using AtlasOps.Features;

public sealed class MetricAlertRecoveryService(
    IAtlasOpsCapabilityRepository<MetricAlertRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricAlertRecoveryValidator validator = new();
    private readonly MetricAlertRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricAlertRecoveryChanged>> ExecuteAsync(
        UpdateMetricAlertRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricAlertRecoveryChanged>.Invalid(issues);
        }

        MetricAlertRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricAlertRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricAlertRecoveryChanged>.Invalid(
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

        MetricAlertRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricAlertRecoveryChanged>.Success(changed);
    }
}