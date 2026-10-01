namespace AtlasOps.Features.Data.DataLineageGovernance;

using AtlasOps.Features;

public sealed class DataLineageGovernanceService(
    IAtlasOpsCapabilityRepository<DataLineageGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataLineageGovernanceValidator validator = new();
    private readonly DataLineageGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataLineageGovernanceChanged>> ExecuteAsync(
        UpdateDataLineageGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataLineageGovernanceChanged>.Invalid(issues);
        }

        DataLineageGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataLineageGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataLineageGovernanceChanged>.Invalid(
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

        DataLineageGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataLineageGovernanceChanged>.Success(changed);
    }
}