namespace AtlasOps.Features.Api.ApiTokenProvisioning;

using AtlasOps.Features;

public sealed class ApiTokenProvisioningService(
    IAtlasOpsCapabilityRepository<ApiTokenProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiTokenProvisioningValidator validator = new();
    private readonly ApiTokenProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiTokenProvisioningChanged>> ExecuteAsync(
        UpdateApiTokenProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiTokenProvisioningChanged>.Invalid(issues);
        }

        ApiTokenProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiTokenProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiTokenProvisioningChanged>.Invalid(
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

        ApiTokenProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiTokenProvisioningChanged>.Success(changed);
    }
}