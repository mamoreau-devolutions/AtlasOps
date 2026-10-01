namespace AtlasOps.Features.Observability.MetricAlertGovernance;

using AtlasOps.Features;

public sealed class MetricAlertGovernanceService(
    IAtlasOpsCapabilityRepository<MetricAlertGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricAlertGovernanceValidator validator = new();
    private readonly MetricAlertGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricAlertGovernanceChanged>> ExecuteAsync(
        UpdateMetricAlertGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricAlertGovernanceChanged>.Invalid(issues);
        }

        MetricAlertGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricAlertGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricAlertGovernanceChanged>.Invalid(
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

        MetricAlertGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricAlertGovernanceChanged>.Success(changed);
    }
}