namespace AtlasOps.Features.Delivery.ReleaseMetricGovernance;

using AtlasOps.Features;

public sealed class ReleaseMetricGovernanceService(
    IAtlasOpsCapabilityRepository<ReleaseMetricGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseMetricGovernanceValidator validator = new();
    private readonly ReleaseMetricGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseMetricGovernanceChanged>> ExecuteAsync(
        UpdateReleaseMetricGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseMetricGovernanceChanged>.Invalid(issues);
        }

        ReleaseMetricGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseMetricGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseMetricGovernanceChanged>.Invalid(
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

        ReleaseMetricGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseMetricGovernanceChanged>.Success(changed);
    }
}