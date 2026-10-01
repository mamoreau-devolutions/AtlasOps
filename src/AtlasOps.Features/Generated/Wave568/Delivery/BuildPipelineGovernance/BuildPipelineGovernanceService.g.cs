namespace AtlasOps.Features.Delivery.BuildPipelineGovernance;

using AtlasOps.Features;

public sealed class BuildPipelineGovernanceService(
    IAtlasOpsCapabilityRepository<BuildPipelineGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildPipelineGovernanceValidator validator = new();
    private readonly BuildPipelineGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildPipelineGovernanceChanged>> ExecuteAsync(
        UpdateBuildPipelineGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildPipelineGovernanceChanged>.Invalid(issues);
        }

        BuildPipelineGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildPipelineGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildPipelineGovernanceChanged>.Invalid(
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

        BuildPipelineGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildPipelineGovernanceChanged>.Success(changed);
    }
}