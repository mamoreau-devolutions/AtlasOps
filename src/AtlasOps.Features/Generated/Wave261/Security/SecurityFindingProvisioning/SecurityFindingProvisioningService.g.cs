namespace AtlasOps.Features.Security.SecurityFindingProvisioning;

using AtlasOps.Features;

public sealed class SecurityFindingProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityFindingProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityFindingProvisioningValidator validator = new();
    private readonly SecurityFindingProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityFindingProvisioningChanged>> ExecuteAsync(
        UpdateSecurityFindingProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityFindingProvisioningChanged>.Invalid(issues);
        }

        SecurityFindingProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityFindingProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityFindingProvisioningChanged>.Invalid(
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

        SecurityFindingProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityFindingProvisioningChanged>.Success(changed);
    }
}