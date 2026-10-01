namespace AtlasOps.Features.Identity.IdentityProviderProvisioning;

using AtlasOps.Features;

public sealed class IdentityProviderProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityProviderProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityProviderProvisioningValidator validator = new();
    private readonly IdentityProviderProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityProviderProvisioningChanged>> ExecuteAsync(
        UpdateIdentityProviderProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityProviderProvisioningChanged>.Invalid(issues);
        }

        IdentityProviderProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityProviderProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityProviderProvisioningChanged>.Invalid(
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

        IdentityProviderProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityProviderProvisioningChanged>.Success(changed);
    }
}