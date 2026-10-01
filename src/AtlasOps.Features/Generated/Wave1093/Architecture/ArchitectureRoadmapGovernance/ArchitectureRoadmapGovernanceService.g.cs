namespace AtlasOps.Features.Architecture.ArchitectureRoadmapGovernance;

using AtlasOps.Features;

public sealed class ArchitectureRoadmapGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureRoadmapGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRoadmapGovernanceValidator validator = new();
    private readonly ArchitectureRoadmapGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRoadmapGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureRoadmapGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapGovernanceChanged>.Invalid(issues);
        }

        ArchitectureRoadmapGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRoadmapGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapGovernanceChanged>.Invalid(
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

        ArchitectureRoadmapGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRoadmapGovernanceChanged>.Success(changed);
    }
}