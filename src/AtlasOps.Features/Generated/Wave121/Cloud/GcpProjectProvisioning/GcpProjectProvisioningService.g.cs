namespace AtlasOps.Features.Cloud.GcpProjectProvisioning;

using AtlasOps.Features;

public sealed class GcpProjectProvisioningService(
    IAtlasOpsCapabilityRepository<GcpProjectProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly GcpProjectProvisioningValidator validator = new();
    private readonly GcpProjectProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GcpProjectProvisioningChanged>> ExecuteAsync(
        UpdateGcpProjectProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GcpProjectProvisioningChanged>.Invalid(issues);
        }

        GcpProjectProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GcpProjectProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GcpProjectProvisioningChanged>.Invalid(
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

        GcpProjectProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GcpProjectProvisioningChanged>.Success(changed);
    }
}