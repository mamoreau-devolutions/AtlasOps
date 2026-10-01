namespace AtlasOps.Features.Api.ApiHealthProvisioning;

using AtlasOps.Features;

public sealed class ApiHealthProvisioningService(
    IAtlasOpsCapabilityRepository<ApiHealthProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiHealthProvisioningValidator validator = new();
    private readonly ApiHealthProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiHealthProvisioningChanged>> ExecuteAsync(
        UpdateApiHealthProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiHealthProvisioningChanged>.Invalid(issues);
        }

        ApiHealthProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiHealthProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiHealthProvisioningChanged>.Invalid(
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

        ApiHealthProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiHealthProvisioningChanged>.Success(changed);
    }
}