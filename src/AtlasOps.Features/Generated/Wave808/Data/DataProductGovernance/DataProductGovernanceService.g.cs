namespace AtlasOps.Features.Data.DataProductGovernance;

using AtlasOps.Features;

public sealed class DataProductGovernanceService(
    IAtlasOpsCapabilityRepository<DataProductGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataProductGovernanceValidator validator = new();
    private readonly DataProductGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataProductGovernanceChanged>> ExecuteAsync(
        UpdateDataProductGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataProductGovernanceChanged>.Invalid(issues);
        }

        DataProductGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataProductGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataProductGovernanceChanged>.Invalid(
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

        DataProductGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataProductGovernanceChanged>.Success(changed);
    }
}