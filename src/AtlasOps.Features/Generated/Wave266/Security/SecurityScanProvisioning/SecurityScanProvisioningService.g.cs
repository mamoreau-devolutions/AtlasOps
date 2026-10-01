namespace AtlasOps.Features.Security.SecurityScanProvisioning;

using AtlasOps.Features;

public sealed class SecurityScanProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityScanProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityScanProvisioningValidator validator = new();
    private readonly SecurityScanProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityScanProvisioningChanged>> ExecuteAsync(
        UpdateSecurityScanProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityScanProvisioningChanged>.Invalid(issues);
        }

        SecurityScanProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityScanProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityScanProvisioningChanged>.Invalid(
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

        SecurityScanProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityScanProvisioningChanged>.Success(changed);
    }
}