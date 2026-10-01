namespace AtlasOps.Features.Observability.ObservabilitySloGovernance;

using AtlasOps.Features;

public sealed class ObservabilitySloGovernanceService(
    IAtlasOpsCapabilityRepository<ObservabilitySloGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilitySloGovernanceValidator validator = new();
    private readonly ObservabilitySloGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilitySloGovernanceChanged>> ExecuteAsync(
        UpdateObservabilitySloGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilitySloGovernanceChanged>.Invalid(issues);
        }

        ObservabilitySloGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilitySloGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilitySloGovernanceChanged>.Invalid(
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

        ObservabilitySloGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilitySloGovernanceChanged>.Success(changed);
    }
}