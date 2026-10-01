namespace AtlasOps.Features.Identity.IdentityUserProvisioning;

using AtlasOps.Features;

public sealed class IdentityUserProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityUserProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityUserProvisioningValidator validator = new();
    private readonly IdentityUserProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityUserProvisioningChanged>> ExecuteAsync(
        UpdateIdentityUserProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityUserProvisioningChanged>.Invalid(issues);
        }

        IdentityUserProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityUserProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityUserProvisioningChanged>.Invalid(
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

        IdentityUserProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityUserProvisioningChanged>.Success(changed);
    }
}