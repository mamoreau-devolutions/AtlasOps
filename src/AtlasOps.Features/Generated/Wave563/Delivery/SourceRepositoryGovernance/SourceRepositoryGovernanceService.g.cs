namespace AtlasOps.Features.Delivery.SourceRepositoryGovernance;

using AtlasOps.Features;

public sealed class SourceRepositoryGovernanceService(
    IAtlasOpsCapabilityRepository<SourceRepositoryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SourceRepositoryGovernanceValidator validator = new();
    private readonly SourceRepositoryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SourceRepositoryGovernanceChanged>> ExecuteAsync(
        UpdateSourceRepositoryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SourceRepositoryGovernanceChanged>.Invalid(issues);
        }

        SourceRepositoryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SourceRepositoryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SourceRepositoryGovernanceChanged>.Invalid(
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

        SourceRepositoryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SourceRepositoryGovernanceChanged>.Success(changed);
    }
}