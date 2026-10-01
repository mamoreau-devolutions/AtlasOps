namespace AtlasOps.Features.Data.DataQualityGovernance;

using AtlasOps.Features;

public sealed class DataQualityGovernanceService(
    IAtlasOpsCapabilityRepository<DataQualityGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataQualityGovernanceValidator validator = new();
    private readonly DataQualityGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataQualityGovernanceChanged>> ExecuteAsync(
        UpdateDataQualityGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataQualityGovernanceChanged>.Invalid(issues);
        }

        DataQualityGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataQualityGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataQualityGovernanceChanged>.Invalid(
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

        DataQualityGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataQualityGovernanceChanged>.Success(changed);
    }
}