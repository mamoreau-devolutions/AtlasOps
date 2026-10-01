namespace AtlasOps.Features.Identity.IdentityFactorProvisioning;

using AtlasOps.Features;

public sealed class IdentityFactorProvisioningService(
    IAtlasOpsCapabilityRepository<IdentityFactorProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityFactorProvisioningValidator validator = new();
    private readonly IdentityFactorProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityFactorProvisioningChanged>> ExecuteAsync(
        UpdateIdentityFactorProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityFactorProvisioningChanged>.Invalid(issues);
        }

        IdentityFactorProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityFactorProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityFactorProvisioningChanged>.Invalid(
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

        IdentityFactorProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityFactorProvisioningChanged>.Success(changed);
    }
}