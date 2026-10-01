namespace AtlasOps.Features.Api.ApiGatewayRecovery;

using AtlasOps.Features;

public sealed class ApiGatewayRecoveryService(
    IAtlasOpsCapabilityRepository<ApiGatewayRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiGatewayRecoveryValidator validator = new();
    private readonly ApiGatewayRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiGatewayRecoveryChanged>> ExecuteAsync(
        UpdateApiGatewayRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiGatewayRecoveryChanged>.Invalid(issues);
        }

        ApiGatewayRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiGatewayRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiGatewayRecoveryChanged>.Invalid(
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

        ApiGatewayRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiGatewayRecoveryChanged>.Success(changed);
    }
}