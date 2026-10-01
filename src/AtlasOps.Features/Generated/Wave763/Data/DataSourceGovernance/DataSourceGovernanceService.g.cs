namespace AtlasOps.Features.Data.DataSourceGovernance;

using AtlasOps.Features;

public sealed class DataSourceGovernanceService(
    IAtlasOpsCapabilityRepository<DataSourceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataSourceGovernanceValidator validator = new();
    private readonly DataSourceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataSourceGovernanceChanged>> ExecuteAsync(
        UpdateDataSourceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataSourceGovernanceChanged>.Invalid(issues);
        }

        DataSourceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataSourceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataSourceGovernanceChanged>.Invalid(
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

        DataSourceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataSourceGovernanceChanged>.Success(changed);
    }
}