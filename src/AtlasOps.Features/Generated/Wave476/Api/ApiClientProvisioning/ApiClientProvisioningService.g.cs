namespace AtlasOps.Features.Api.ApiClientProvisioning;

using AtlasOps.Features;

public sealed class ApiClientProvisioningService(
    IAtlasOpsCapabilityRepository<ApiClientProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiClientProvisioningValidator validator = new();
    private readonly ApiClientProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiClientProvisioningChanged>> ExecuteAsync(
        UpdateApiClientProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiClientProvisioningChanged>.Invalid(issues);
        }

        ApiClientProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiClientProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiClientProvisioningChanged>.Invalid(
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

        ApiClientProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiClientProvisioningChanged>.Success(changed);
    }
}