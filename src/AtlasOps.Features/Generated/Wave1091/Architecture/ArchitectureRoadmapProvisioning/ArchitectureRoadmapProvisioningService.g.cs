namespace AtlasOps.Features.Architecture.ArchitectureRoadmapProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureRoadmapProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureRoadmapProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRoadmapProvisioningValidator validator = new();
    private readonly ArchitectureRoadmapProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRoadmapProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureRoadmapProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapProvisioningChanged>.Invalid(issues);
        }

        ArchitectureRoadmapProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRoadmapProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRoadmapProvisioningChanged>.Invalid(
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

        ArchitectureRoadmapProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRoadmapProvisioningChanged>.Success(changed);
    }
}