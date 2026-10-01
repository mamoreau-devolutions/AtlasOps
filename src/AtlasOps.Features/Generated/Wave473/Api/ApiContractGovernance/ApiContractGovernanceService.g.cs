namespace AtlasOps.Features.Api.ApiContractGovernance;

using AtlasOps.Features;

public sealed class ApiContractGovernanceService(
    IAtlasOpsCapabilityRepository<ApiContractGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApiContractGovernanceValidator validator = new();
    private readonly ApiContractGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApiContractGovernanceChanged>> ExecuteAsync(
        UpdateApiContractGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApiContractGovernanceChanged>.Invalid(issues);
        }

        ApiContractGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApiContractGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApiContractGovernanceChanged>.Invalid(
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

        ApiContractGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApiContractGovernanceChanged>.Success(changed);
    }
}