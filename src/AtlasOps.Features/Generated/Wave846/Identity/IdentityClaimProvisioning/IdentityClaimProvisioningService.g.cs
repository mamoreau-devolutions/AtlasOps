namespace AtlasOps.Features.Identity.IdentityClaimProvisioning;

using AtlasOps.Features;

public sealed class IdentityClaimProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityClaimProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityClaimProvisioningValidator validator = new();
    private readonly IdentityClaimProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityClaimProvisioningChanged>> ExecuteAsync(
        UpdateIdentityClaimProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityClaimProvisioningChanged>.Invalid(issues);
        }

        IdentityClaimProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityClaimProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityClaimProvisioningChanged>.Invalid(
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

        IdentityClaimProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityClaimProvisioningChanged>.Success(changed);
    }
}