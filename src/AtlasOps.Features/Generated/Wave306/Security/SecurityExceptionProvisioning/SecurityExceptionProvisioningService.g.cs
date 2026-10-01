namespace AtlasOps.Features.Security.SecurityExceptionProvisioning;

using AtlasOps.Features;

public sealed class SecurityExceptionProvisioningService(
    IAtlasOpsCapabilityRepository<SecurityExceptionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityExceptionProvisioningValidator validator = new();
    private readonly SecurityExceptionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityExceptionProvisioningChanged>> ExecuteAsync(
        UpdateSecurityExceptionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityExceptionProvisioningChanged>.Invalid(issues);
        }

        SecurityExceptionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityExceptionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityExceptionProvisioningChanged>.Invalid(
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

        SecurityExceptionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityExceptionProvisioningChanged>.Success(changed);
    }
}