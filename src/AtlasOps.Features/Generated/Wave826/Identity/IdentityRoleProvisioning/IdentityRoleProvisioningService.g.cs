namespace AtlasOps.Features.Identity.IdentityRoleProvisioning;

using AtlasOps.Features;

public sealed class IdentityRoleProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityRoleProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityRoleProvisioningValidator validator = new();
    private readonly IdentityRoleProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityRoleProvisioningChanged>> ExecuteAsync(
        UpdateIdentityRoleProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityRoleProvisioningChanged>.Invalid(issues);
        }

        IdentityRoleProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityRoleProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityRoleProvisioningChanged>.Invalid(
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

        IdentityRoleProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityRoleProvisioningChanged>.Success(changed);
    }
}