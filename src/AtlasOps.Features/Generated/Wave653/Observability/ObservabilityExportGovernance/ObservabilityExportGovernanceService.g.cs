namespace AtlasOps.Features.Observability.ObservabilityExportGovernance;

using AtlasOps.Features;

public sealed class ObservabilityExportGovernanceService(
    IAtlasOpsCapabilityRepository<ObservabilityExportGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityExportGovernanceValidator validator = new();
    private readonly ObservabilityExportGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityExportGovernanceChanged>> ExecuteAsync(
        UpdateObservabilityExportGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityExportGovernanceChanged>.Invalid(issues);
        }

        ObservabilityExportGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityExportGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityExportGovernanceChanged>.Invalid(
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

        ObservabilityExportGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityExportGovernanceChanged>.Success(changed);
    }
}