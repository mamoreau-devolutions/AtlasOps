namespace AtlasOps.Features.Delivery.ReleaseGateProvisioning;

using AtlasOps.Features;

public sealed class ReleaseGateProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseGateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseGateProvisioningValidator validator = new();
    private readonly ReleaseGateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseGateProvisioningChanged>> ExecuteAsync(
        UpdateReleaseGateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseGateProvisioningChanged>.Invalid(issues);
        }

        ReleaseGateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseGateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseGateProvisioningChanged>.Invalid(
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

        ReleaseGateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseGateProvisioningChanged>.Success(changed);
    }
}