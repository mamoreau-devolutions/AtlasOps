namespace AtlasOps.Features.Compute.ComputeImageProvisioning;

using AtlasOps.Features;

public sealed class ComputeImageProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeImageProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeImageProvisioningValidator validator = new();
    private readonly ComputeImageProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeImageProvisioningChanged>> ExecuteAsync(
        UpdateComputeImageProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeImageProvisioningChanged>.Invalid(issues);
        }

        ComputeImageProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeImageProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeImageProvisioningChanged>.Invalid(
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

        ComputeImageProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeImageProvisioningChanged>.Success(changed);
    }
}