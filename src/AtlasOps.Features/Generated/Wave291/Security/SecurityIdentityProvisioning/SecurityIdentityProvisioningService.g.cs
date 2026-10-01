namespace AtlasOps.Features.Security.SecurityIdentityProvisioning;

using AtlasOps.Features;

public sealed class SecurityIdentityProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityIdentityProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityIdentityProvisioningValidator validator = new();
    private readonly SecurityIdentityProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityIdentityProvisioningChanged>> ExecuteAsync(
        UpdateSecurityIdentityProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityIdentityProvisioningChanged>.Invalid(issues);
        }

        SecurityIdentityProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityIdentityProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityIdentityProvisioningChanged>.Invalid(
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

        SecurityIdentityProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityIdentityProvisioningChanged>.Success(changed);
    }
}