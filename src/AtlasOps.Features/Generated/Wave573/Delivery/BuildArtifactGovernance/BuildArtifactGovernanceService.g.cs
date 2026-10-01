namespace AtlasOps.Features.Delivery.BuildArtifactGovernance;

using AtlasOps.Features;

public sealed class BuildArtifactGovernanceService(
    IAtlasOpsCapabilityRepository<BuildArtifactGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildArtifactGovernanceValidator validator = new();
    private readonly BuildArtifactGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildArtifactGovernanceChanged>> ExecuteAsync(
        UpdateBuildArtifactGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildArtifactGovernanceChanged>.Invalid(issues);
        }

        BuildArtifactGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildArtifactGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildArtifactGovernanceChanged>.Invalid(
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

        BuildArtifactGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildArtifactGovernanceChanged>.Success(changed);
    }
}