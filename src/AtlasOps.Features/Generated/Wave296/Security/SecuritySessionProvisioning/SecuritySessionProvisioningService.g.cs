namespace AtlasOps.Features.Security.SecuritySessionProvisioning;

using AtlasOps.Features;

public sealed class SecuritySessionProvisioningService(
    IAtlasOpsCapabilityRepository<SecuritySessionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecuritySessionProvisioningValidator validator = new();
    private readonly SecuritySessionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecuritySessionProvisioningChanged>> ExecuteAsync(
        UpdateSecuritySessionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecuritySessionProvisioningChanged>.Invalid(issues);
        }

        SecuritySessionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecuritySessionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecuritySessionProvisioningChanged>.Invalid(
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

        SecuritySessionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecuritySessionProvisioningChanged>.Success(changed);
    }
}