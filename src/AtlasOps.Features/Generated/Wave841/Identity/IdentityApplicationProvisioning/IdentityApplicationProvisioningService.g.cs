namespace AtlasOps.Features.Identity.IdentityApplicationProvisioning;

using AtlasOps.Features;

public sealed class IdentityApplicationProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityApplicationProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityApplicationProvisioningValidator validator = new();
    private readonly IdentityApplicationProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityApplicationProvisioningChanged>> ExecuteAsync(
        UpdateIdentityApplicationProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityApplicationProvisioningChanged>.Invalid(issues);
        }

        IdentityApplicationProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityApplicationProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityApplicationProvisioningChanged>.Invalid(
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

        IdentityApplicationProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityApplicationProvisioningChanged>.Success(changed);
    }
}