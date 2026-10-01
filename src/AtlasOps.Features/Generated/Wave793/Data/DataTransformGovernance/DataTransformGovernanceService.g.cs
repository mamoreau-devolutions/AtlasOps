namespace AtlasOps.Features.Data.DataTransformGovernance;

using AtlasOps.Features;

public sealed class DataTransformGovernanceService(
    IAtlasOpsCapabilityRepository<DataTransformGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataTransformGovernanceValidator validator = new();
    private readonly DataTransformGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataTransformGovernanceChanged>> ExecuteAsync(
        UpdateDataTransformGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataTransformGovernanceChanged>.Invalid(issues);
        }

        DataTransformGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataTransformGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataTransformGovernanceChanged>.Invalid(
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

        DataTransformGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataTransformGovernanceChanged>.Success(changed);
    }
}