namespace AtlasOps.Features.Api.ApiGatewayGovernance;

using AtlasOps.Features;

public sealed class ApiGatewayGovernanceService(
    IAtlasOpsCapabilityRepository<ApiGatewayGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiGatewayGovernanceValidator validator = new();
    private readonly ApiGatewayGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiGatewayGovernanceChanged>> ExecuteAsync(
        UpdateApiGatewayGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiGatewayGovernanceChanged>.Invalid(issues);
        }

        ApiGatewayGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiGatewayGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiGatewayGovernanceChanged>.Invalid(
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

        ApiGatewayGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiGatewayGovernanceChanged>.Success(changed);
    }
}