namespace AtlasOps.Features.Architecture.ArchitectureRoadmapOptimization;

using AtlasOps.Features;

public sealed class ArchitectureRoadmapOptimizationService(
    IAtlasOpsCapabilityRepository<ArchitectureRoadmapOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRoadmapOptimizationValidator validator = new();
    private readonly ArchitectureRoadmapOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRoadmapOptimizationChanged>> ExecuteAsync(
        UpdateArchitectureRoadmapOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapOptimizationChanged>.Invalid(issues);
        }

        ArchitectureRoadmapOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRoadmapOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapOptimizationChanged>.Invalid(
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

        ArchitectureRoadmapOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRoadmapOptimizationChanged>.Success(changed);
    }
}