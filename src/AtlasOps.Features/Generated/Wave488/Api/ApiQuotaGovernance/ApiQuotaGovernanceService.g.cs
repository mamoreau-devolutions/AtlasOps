namespace AtlasOps.Features.Api.ApiQuotaGovernance;

using AtlasOps.Features;

public sealed class ApiQuotaGovernanceService(
    IAtlasOpsCapabilityRepository<ApiQuotaGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiQuotaGovernanceValidator validator = new();
    private readonly ApiQuotaGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiQuotaGovernanceChanged>> ExecuteAsync(
        UpdateApiQuotaGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiQuotaGovernanceChanged>.Invalid(issues);
        }

        ApiQuotaGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiQuotaGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiQuotaGovernanceChanged>.Invalid(
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

        ApiQuotaGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiQuotaGovernanceChanged>.Success(changed);
    }
}