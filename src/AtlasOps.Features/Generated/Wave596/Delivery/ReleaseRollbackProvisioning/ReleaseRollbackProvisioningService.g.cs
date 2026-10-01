namespace AtlasOps.Features.Delivery.ReleaseRollbackProvisioning;

using AtlasOps.Features;

public sealed class ReleaseRollbackProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseRollbackProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseRollbackProvisioningValidator validator = new();
    private readonly ReleaseRollbackProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseRollbackProvisioningChanged>> ExecuteAsync(
        UpdateReleaseRollbackProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseRollbackProvisioningChanged>.Invalid(issues);
        }

        ReleaseRollbackProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseRollbackProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseRollbackProvisioningChanged>.Invalid(
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

        ReleaseRollbackProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseRollbackProvisioningChanged>.Success(changed);
    }
}