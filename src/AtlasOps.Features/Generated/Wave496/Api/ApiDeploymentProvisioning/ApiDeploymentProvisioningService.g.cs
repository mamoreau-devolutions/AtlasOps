namespace AtlasOps.Features.Api.ApiDeploymentProvisioning;

using AtlasOps.Features;

public sealed class ApiDeploymentProvisioningService(
    IAtlasOpsCapabilityRepository<ApiDeploymentProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiDeploymentProvisioningValidator validator = new();
    private readonly ApiDeploymentProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiDeploymentProvisioningChanged>> ExecuteAsync(
        UpdateApiDeploymentProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiDeploymentProvisioningChanged>.Invalid(issues);
        }

        ApiDeploymentProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiDeploymentProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiDeploymentProvisioningChanged>.Invalid(
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

        ApiDeploymentProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiDeploymentProvisioningChanged>.Success(changed);
    }
}