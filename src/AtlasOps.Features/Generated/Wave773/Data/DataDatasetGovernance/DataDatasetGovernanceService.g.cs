namespace AtlasOps.Features.Data.DataDatasetGovernance;

using AtlasOps.Features;

public sealed class DataDatasetGovernanceService(
    IAtlasOpsCapabilityRepository<DataDatasetGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataDatasetGovernanceValidator validator = new();
    private readonly DataDatasetGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataDatasetGovernanceChanged>> ExecuteAsync(
        UpdateDataDatasetGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataDatasetGovernanceChanged>.Invalid(issues);
        }

        DataDatasetGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataDatasetGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataDatasetGovernanceChanged>.Invalid(
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

        DataDatasetGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataDatasetGovernanceChanged>.Success(changed);
    }
}