namespace AtlasOps.Features.Api.ApiEndpointGovernance;

using AtlasOps.Features;

public sealed class ApiEndpointGovernanceService(
    IAtlasOpsCapabilityRepository<ApiEndpointGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiEndpointGovernanceValidator validator = new();
    private readonly ApiEndpointGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiEndpointGovernanceChanged>> ExecuteAsync(
        UpdateApiEndpointGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiEndpointGovernanceChanged>.Invalid(issues);
        }

        ApiEndpointGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiEndpointGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiEndpointGovernanceChanged>.Invalid(
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

        ApiEndpointGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiEndpointGovernanceChanged>.Success(changed);
    }
}