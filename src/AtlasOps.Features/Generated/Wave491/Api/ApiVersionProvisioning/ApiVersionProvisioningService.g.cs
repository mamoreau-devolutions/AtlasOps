namespace AtlasOps.Features.Api.ApiVersionProvisioning;

using AtlasOps.Features;

public sealed class ApiVersionProvisioningService(
    IAtlasOpsCapabilityRepository<ApiVersionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiVersionProvisioningValidator validator = new();
    private readonly ApiVersionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiVersionProvisioningChanged>> ExecuteAsync(
        UpdateApiVersionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiVersionProvisioningChanged>.Invalid(issues);
        }

        ApiVersionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiVersionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiVersionProvisioningChanged>.Invalid(
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

        ApiVersionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiVersionProvisioningChanged>.Success(changed);
    }
}