namespace AtlasOps.Features.Security.SecurityBaselineProvisioning;

using AtlasOps.Features;

public sealed class SecurityBaselineProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityBaselineProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBaselineProvisioningValidator validator = new();
    private readonly SecurityBaselineProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBaselineProvisioningChanged>> ExecuteAsync(
        UpdateSecurityBaselineProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBaselineProvisioningChanged>.Invalid(issues);
        }

        SecurityBaselineProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBaselineProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBaselineProvisioningChanged>.Invalid(
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

        SecurityBaselineProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBaselineProvisioningChanged>.Success(changed);
    }
}