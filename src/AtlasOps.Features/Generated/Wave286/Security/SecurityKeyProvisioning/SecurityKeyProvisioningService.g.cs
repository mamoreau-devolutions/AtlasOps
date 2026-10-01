namespace AtlasOps.Features.Security.SecurityKeyProvisioning;

using AtlasOps.Features;

public sealed class SecurityKeyProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityKeyProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityKeyProvisioningValidator validator = new();
    private readonly SecurityKeyProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityKeyProvisioningChanged>> ExecuteAsync(
        UpdateSecurityKeyProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityKeyProvisioningChanged>.Invalid(issues);
        }

        SecurityKeyProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityKeyProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityKeyProvisioningChanged>.Invalid(
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

        SecurityKeyProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityKeyProvisioningChanged>.Success(changed);
    }
}