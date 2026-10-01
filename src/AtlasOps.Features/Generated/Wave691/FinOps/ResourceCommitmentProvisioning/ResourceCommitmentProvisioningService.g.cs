namespace AtlasOps.Features.FinOps.ResourceCommitmentProvisioning;

using AtlasOps.Features;

public sealed class ResourceCommitmentProvisioningService(
    IAtlasOpsCapabilityRepository<ResourceCommitmentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResourceCommitmentProvisioningValidator validator = new();
    private readonly ResourceCommitmentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResourceCommitmentProvisioningChanged>> ExecuteAsync(
        UpdateResourceCommitmentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResourceCommitmentProvisioningChanged>.Invalid(issues);
        }

        ResourceCommitmentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResourceCommitmentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResourceCommitmentProvisioningChanged>.Invalid(
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

        ResourceCommitmentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResourceCommitmentProvisioningChanged>.Success(changed);
    }
}