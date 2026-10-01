namespace AtlasOps.Features.Compute.ComputeScaleSetProvisioning;

using AtlasOps.Features;

public sealed class ComputeScaleSetProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeScaleSetProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScaleSetProvisioningValidator validator = new();
    private readonly ComputeScaleSetProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScaleSetProvisioningChanged>> ExecuteAsync(
        UpdateComputeScaleSetProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScaleSetProvisioningChanged>.Invalid(issues);
        }

        ComputeScaleSetProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScaleSetProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScaleSetProvisioningChanged>.Invalid(
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

        ComputeScaleSetProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScaleSetProvisioningChanged>.Success(changed);
    }
}