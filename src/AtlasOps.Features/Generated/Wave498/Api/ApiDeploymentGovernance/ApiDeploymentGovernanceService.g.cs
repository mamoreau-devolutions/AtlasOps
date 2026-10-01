namespace AtlasOps.Features.Api.ApiDeploymentGovernance;

using AtlasOps.Features;

public sealed class ApiDeploymentGovernanceService(
    IAtlasOpsCapabilityRepository<ApiDeploymentGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiDeploymentGovernanceValidator validator = new();
    private readonly ApiDeploymentGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiDeploymentGovernanceChanged>> ExecuteAsync(
        UpdateApiDeploymentGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiDeploymentGovernanceChanged>.Invalid(issues);
        }

        ApiDeploymentGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiDeploymentGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiDeploymentGovernanceChanged>.Invalid(
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

        ApiDeploymentGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiDeploymentGovernanceChanged>.Success(changed);
    }
}