namespace AtlasOps.Features.Observability.ObservabilityDashboardGovernance;

using AtlasOps.Features;

public sealed class ObservabilityDashboardGovernanceService(
    IAtlasOpsCapabilityRepository<ObservabilityDashboardGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityDashboardGovernanceValidator validator = new();
    private readonly ObservabilityDashboardGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityDashboardGovernanceChanged>> ExecuteAsync(
        UpdateObservabilityDashboardGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityDashboardGovernanceChanged>.Invalid(issues);
        }

        ObservabilityDashboardGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityDashboardGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityDashboardGovernanceChanged>.Invalid(
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

        ObservabilityDashboardGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityDashboardGovernanceChanged>.Success(changed);
    }
}