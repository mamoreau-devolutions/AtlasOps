namespace AtlasOps.Features.Identity.IdentityLifecycleProvisioning;

using AtlasOps.Features;

public sealed class IdentityLifecycleProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityLifecycleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityLifecycleProvisioningValidator validator = new();
    private readonly IdentityLifecycleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityLifecycleProvisioningChanged>> ExecuteAsync(
        UpdateIdentityLifecycleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityLifecycleProvisioningChanged>.Invalid(issues);
        }

        IdentityLifecycleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityLifecycleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityLifecycleProvisioningChanged>.Invalid(
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

        IdentityLifecycleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityLifecycleProvisioningChanged>.Success(changed);
    }
}