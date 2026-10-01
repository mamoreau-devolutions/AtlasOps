namespace AtlasOps.Features.Identity.IdentityAuditProvisioning;

using AtlasOps.Features;

public sealed class IdentityAuditProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityAuditProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityAuditProvisioningValidator validator = new();
    private readonly IdentityAuditProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityAuditProvisioningChanged>> ExecuteAsync(
        UpdateIdentityAuditProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityAuditProvisioningChanged>.Invalid(issues);
        }

        IdentityAuditProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityAuditProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityAuditProvisioningChanged>.Invalid(
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

        IdentityAuditProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityAuditProvisioningChanged>.Success(changed);
    }
}