namespace AtlasOps.Features.Observability.MetricSourceGovernance;

using AtlasOps.Features;

public sealed class MetricSourceGovernanceService(
    IAtlasOpsCapabilityRepository<MetricSourceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricSourceGovernanceValidator validator = new();
    private readonly MetricSourceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricSourceGovernanceChanged>> ExecuteAsync(
        UpdateMetricSourceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricSourceGovernanceChanged>.Invalid(issues);
        }

        MetricSourceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricSourceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricSourceGovernanceChanged>.Invalid(
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

        MetricSourceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricSourceGovernanceChanged>.Success(changed);
    }
}