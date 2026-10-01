namespace AtlasOps.Features.Compute.ComputeMetricProvisioning;

using AtlasOps.Features;

public sealed class ComputeMetricProvisioningService(
    IAtlasOpsCapabilityRepository<ComputeMetricProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeMetricProvisioningValidator validator = new();
    private readonly ComputeMetricProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeMetricProvisioningChanged>> ExecuteAsync(
        UpdateComputeMetricProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeMetricProvisioningChanged>.Invalid(issues);
        }

        ComputeMetricProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeMetricProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeMetricProvisioningChanged>.Invalid(
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

        ComputeMetricProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeMetricProvisioningChanged>.Success(changed);
    }
}