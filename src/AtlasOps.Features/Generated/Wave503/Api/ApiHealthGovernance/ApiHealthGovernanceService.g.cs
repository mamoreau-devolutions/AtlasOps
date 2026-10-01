namespace AtlasOps.Features.Api.ApiHealthGovernance;

using AtlasOps.Features;

public sealed class ApiHealthGovernanceService(
    IAtlasOpsCapabilityRepository<ApiHealthGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiHealthGovernanceValidator validator = new();
    private readonly ApiHealthGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiHealthGovernanceChanged>> ExecuteAsync(
        UpdateApiHealthGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiHealthGovernanceChanged>.Invalid(issues);
        }

        ApiHealthGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiHealthGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiHealthGovernanceChanged>.Invalid(
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

        ApiHealthGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiHealthGovernanceChanged>.Success(changed);
    }
}