namespace AtlasOps.Features.Delivery.BuildArtifactOptimization;

using AtlasOps.Features;

public sealed class BuildArtifactOptimizationService(
    IAtlasOpsCapabilityRepository<BuildArtifactOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildArtifactOptimizationValidator validator = new();
    private readonly BuildArtifactOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildArtifactOptimizationChanged>> ExecuteAsync(
        UpdateBuildArtifactOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildArtifactOptimizationChanged>.Invalid(issues);
        }

        BuildArtifactOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildArtifactOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildArtifactOptimizationChanged>.Invalid(
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

        BuildArtifactOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildArtifactOptimizationChanged>.Success(changed);
    }
}