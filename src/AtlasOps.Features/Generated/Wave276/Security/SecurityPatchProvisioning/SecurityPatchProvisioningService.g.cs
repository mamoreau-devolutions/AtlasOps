namespace AtlasOps.Features.Security.SecurityPatchProvisioning;

using AtlasOps.Features;

public sealed class SecurityPatchProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityPatchProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityPatchProvisioningValidator validator = new();
    private readonly SecurityPatchProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityPatchProvisioningChanged>> ExecuteAsync(
        UpdateSecurityPatchProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityPatchProvisioningChanged>.Invalid(issues);
        }

        SecurityPatchProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityPatchProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityPatchProvisioningChanged>.Invalid(
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

        SecurityPatchProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityPatchProvisioningChanged>.Success(changed);
    }
}