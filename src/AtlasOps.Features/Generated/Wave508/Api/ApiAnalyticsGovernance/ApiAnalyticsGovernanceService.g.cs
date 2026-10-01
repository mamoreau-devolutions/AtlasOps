namespace AtlasOps.Features.Api.ApiAnalyticsGovernance;

using AtlasOps.Features;

public sealed class ApiAnalyticsGovernanceService(
    IAtlasOpsCapabilityRepository<ApiAnalyticsGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiAnalyticsGovernanceValidator validator = new();
    private readonly ApiAnalyticsGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiAnalyticsGovernanceChanged>> ExecuteAsync(
        UpdateApiAnalyticsGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiAnalyticsGovernanceChanged>.Invalid(issues);
        }

        ApiAnalyticsGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiAnalyticsGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiAnalyticsGovernanceChanged>.Invalid(
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

        ApiAnalyticsGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiAnalyticsGovernanceChanged>.Success(changed);
    }
}