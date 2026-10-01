namespace AtlasOps.Features.Delivery.ReleaseCalendarProvisioning;

using AtlasOps.Features;

public sealed class ReleaseCalendarProvisioningService(
    IAtlasOpsCapabilityRepository<ReleaseCalendarProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseCalendarProvisioningValidator validator = new();
    private readonly ReleaseCalendarProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseCalendarProvisioningChanged>> ExecuteAsync(
        UpdateReleaseCalendarProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseCalendarProvisioningChanged>.Invalid(issues);
        }

        ReleaseCalendarProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseCalendarProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseCalendarProvisioningChanged>.Invalid(
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

        ReleaseCalendarProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseCalendarProvisioningChanged>.Success(changed);
    }
}