namespace AtlasOps.Features.Api.ApiEndpointProvisioning;

using AtlasOps.Features;

public sealed class ApiEndpointProvisioningService(
    IAtlasOpsCapabilityRepository<ApiEndpointProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiEndpointProvisioningValidator validator = new();
    private readonly ApiEndpointProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiEndpointProvisioningChanged>> ExecuteAsync(
        UpdateApiEndpointProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiEndpointProvisioningChanged>.Invalid(issues);
        }

        ApiEndpointProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiEndpointProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiEndpointProvisioningChanged>.Invalid(
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

        ApiEndpointProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiEndpointProvisioningChanged>.Success(changed);
    }
}