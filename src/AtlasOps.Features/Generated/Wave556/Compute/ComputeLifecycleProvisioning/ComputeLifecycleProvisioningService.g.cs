namespace AtlasOps.Features.Compute.ComputeLifecycleProvisioning;

using AtlasOps.Features;

public sealed class ComputeLifecycleProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeLifecycleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeLifecycleProvisioningValidator validator = new();
    private readonly ComputeLifecycleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeLifecycleProvisioningChanged>> ExecuteAsync(
        UpdateComputeLifecycleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeLifecycleProvisioningChanged>.Invalid(issues);
        }

        ComputeLifecycleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeLifecycleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeLifecycleProvisioningChanged>.Invalid(
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

        ComputeLifecycleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeLifecycleProvisioningChanged>.Success(changed);
    }
}