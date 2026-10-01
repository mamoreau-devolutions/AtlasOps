namespace AtlasOps.Features.ServiceManagement.ChangeRequestProvisioning;

using AtlasOps.Features;

public sealed class ChangeRequestProvisioningService(
    IAtlasOpsCapabilityRepository<ChangeRequestProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChangeRequestProvisioningValidator validator = new();
    private readonly ChangeRequestProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChangeRequestProvisioningChanged>> ExecuteAsync(
        UpdateChangeRequestProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChangeRequestProvisioningChanged>.Invalid(issues);
        }

        ChangeRequestProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChangeRequestProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChangeRequestProvisioningChanged>.Invalid(
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

        ChangeRequestProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChangeRequestProvisioningChanged>.Success(changed);
    }
}