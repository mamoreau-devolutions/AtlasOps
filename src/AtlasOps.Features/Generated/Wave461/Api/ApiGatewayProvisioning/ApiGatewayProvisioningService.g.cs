namespace AtlasOps.Features.Api.ApiGatewayProvisioning;

using AtlasOps.Features;

public sealed class ApiGatewayProvisioningService(
    IAtlasOpsCapabilityRepository<ApiGatewayProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiGatewayProvisioningValidator validator = new();
    private readonly ApiGatewayProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiGatewayProvisioningChanged>> ExecuteAsync(
        UpdateApiGatewayProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiGatewayProvisioningChanged>.Invalid(issues);
        }

        ApiGatewayProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiGatewayProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiGatewayProvisioningChanged>.Invalid(
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

        ApiGatewayProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiGatewayProvisioningChanged>.Success(changed);
    }
}