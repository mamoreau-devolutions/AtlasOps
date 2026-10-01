namespace AtlasOps.Features.Delivery.BuildPipelineProvisioning;

using AtlasOps.Features;

public sealed class BuildPipelineProvisioningService(
    IAtlasOpsCapabilityRepository<BuildPipelineProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly BuildPipelineProvisioningValidator validator = new();
    private readonly BuildPipelineProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<BuildPipelineProvisioningChanged>> ExecuteAsync(
        UpdateBuildPipelineProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<BuildPipelineProvisioningChanged>.Invalid(issues);
        }

        BuildPipelineProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new BuildPipelineProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<BuildPipelineProvisioningChanged>.Invalid(
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

        BuildPipelineProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<BuildPipelineProvisioningChanged>.Success(changed);
    }
}