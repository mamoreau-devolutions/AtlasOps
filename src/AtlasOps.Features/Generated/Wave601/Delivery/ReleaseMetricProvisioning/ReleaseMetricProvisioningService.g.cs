namespace AtlasOps.Features.Delivery.ReleaseMetricProvisioning;

using AtlasOps.Features;

public sealed class ReleaseMetricProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseMetricProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseMetricProvisioningValidator validator = new();
    private readonly ReleaseMetricProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseMetricProvisioningChanged>> ExecuteAsync(
        UpdateReleaseMetricProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseMetricProvisioningChanged>.Invalid(issues);
        }

        ReleaseMetricProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseMetricProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseMetricProvisioningChanged>.Invalid(
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

        ReleaseMetricProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseMetricProvisioningChanged>.Success(changed);
    }
}