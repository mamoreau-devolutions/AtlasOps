namespace AtlasOps.Features.Delivery.ReleaseEnvironmentProvisioning;

using AtlasOps.Features;

public sealed class ReleaseEnvironmentProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseEnvironmentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseEnvironmentProvisioningValidator validator = new();
    private readonly ReleaseEnvironmentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseEnvironmentProvisioningChanged>> ExecuteAsync(
        UpdateReleaseEnvironmentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseEnvironmentProvisioningChanged>.Invalid(issues);
        }

        ReleaseEnvironmentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseEnvironmentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseEnvironmentProvisioningChanged>.Invalid(
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

        ReleaseEnvironmentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseEnvironmentProvisioningChanged>.Success(changed);
    }
}