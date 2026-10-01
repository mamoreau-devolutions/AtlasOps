namespace AtlasOps.Features.Delivery.BuildArtifactProvisioning;

using AtlasOps.Features;

public sealed class BuildArtifactProvisioningService(
    IAtlasOpsCapabilityRepository<BuildArtifactProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildArtifactProvisioningValidator validator = new();
    private readonly BuildArtifactProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildArtifactProvisioningChanged>> ExecuteAsync(
        UpdateBuildArtifactProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildArtifactProvisioningChanged>.Invalid(issues);
        }

        BuildArtifactProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildArtifactProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildArtifactProvisioningChanged>.Invalid(
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

        BuildArtifactProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildArtifactProvisioningChanged>.Success(changed);
    }
}