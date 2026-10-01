namespace AtlasOps.Features.Mobile.MobileTelemetryProvisioning;

using AtlasOps.Features;

public sealed class MobileTelemetryProvisioningService(
    IAtlasOpsCapabilityRepository<MobileTelemetryProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileTelemetryProvisioningValidator validator = new();
    private readonly MobileTelemetryProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileTelemetryProvisioningChanged>> ExecuteAsync(
        UpdateMobileTelemetryProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileTelemetryProvisioningChanged>.Invalid(issues);
        }

        MobileTelemetryProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileTelemetryProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileTelemetryProvisioningChanged>.Invalid(
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

        MobileTelemetryProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileTelemetryProvisioningChanged>.Success(changed);
    }
}