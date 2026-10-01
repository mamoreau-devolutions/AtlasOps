namespace AtlasOps.Features.Security.SecurityBoundaryProvisioning;

using AtlasOps.Features;

public sealed class SecurityBoundaryProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityBoundaryProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBoundaryProvisioningValidator validator = new();
    private readonly SecurityBoundaryProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBoundaryProvisioningChanged>> ExecuteAsync(
        UpdateSecurityBoundaryProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBoundaryProvisioningChanged>.Invalid(issues);
        }

        SecurityBoundaryProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBoundaryProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBoundaryProvisioningChanged>.Invalid(
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

        SecurityBoundaryProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBoundaryProvisioningChanged>.Success(changed);
    }
}