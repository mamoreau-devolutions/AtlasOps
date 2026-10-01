namespace AtlasOps.Features.Architecture.ArchitectureRoadmapRecovery;

using AtlasOps.Features;

public sealed class ArchitectureRoadmapRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureRoadmapRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRoadmapRecoveryValidator validator = new();
    private readonly ArchitectureRoadmapRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRoadmapRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureRoadmapRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapRecoveryChanged>.Invalid(issues);
        }

        ArchitectureRoadmapRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRoadmapRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapRecoveryChanged>.Invalid(
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

        ArchitectureRoadmapRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRoadmapRecoveryChanged>.Success(changed);
    }
}