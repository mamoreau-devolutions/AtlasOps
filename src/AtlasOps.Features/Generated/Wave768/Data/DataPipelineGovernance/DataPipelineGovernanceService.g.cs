namespace AtlasOps.Features.Data.DataPipelineGovernance;

using AtlasOps.Features;

public sealed class DataPipelineGovernanceService(
    IAtlasOpsCapabilityRepository<DataPipelineGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataPipelineGovernanceValidator validator = new();
    private readonly DataPipelineGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataPipelineGovernanceChanged>> ExecuteAsync(
        UpdateDataPipelineGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataPipelineGovernanceChanged>.Invalid(issues);
        }

        DataPipelineGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataPipelineGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataPipelineGovernanceChanged>.Invalid(
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

        DataPipelineGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataPipelineGovernanceChanged>.Success(changed);
    }
}