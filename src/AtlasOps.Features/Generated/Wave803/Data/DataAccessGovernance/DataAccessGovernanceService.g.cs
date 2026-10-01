namespace AtlasOps.Features.Data.DataAccessGovernance;

using AtlasOps.Features;

public sealed class DataAccessGovernanceService(
    IAtlasOpsCapabilityRepository<DataAccessGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataAccessGovernanceValidator validator = new();
    private readonly DataAccessGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataAccessGovernanceChanged>> ExecuteAsync(
        UpdateDataAccessGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataAccessGovernanceChanged>.Invalid(issues);
        }

        DataAccessGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataAccessGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataAccessGovernanceChanged>.Invalid(
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

        DataAccessGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataAccessGovernanceChanged>.Success(changed);
    }
}