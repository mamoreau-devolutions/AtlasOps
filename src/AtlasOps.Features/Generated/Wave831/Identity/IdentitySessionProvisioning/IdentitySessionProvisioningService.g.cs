namespace AtlasOps.Features.Identity.IdentitySessionProvisioning;

using AtlasOps.Features;

public sealed class IdentitySessionProvisioningService(
    IAtlasOpsCapabilityRepository<IdentitySessionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentitySessionProvisioningValidator validator = new();
    private readonly IdentitySessionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentitySessionProvisioningChanged>> ExecuteAsync(
        UpdateIdentitySessionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentitySessionProvisioningChanged>.Invalid(issues);
        }

        IdentitySessionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentitySessionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentitySessionProvisioningChanged>.Invalid(
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

        IdentitySessionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentitySessionProvisioningChanged>.Success(changed);
    }
}