namespace AtlasOps.Features.Data.DataContractGovernance;

using AtlasOps.Features;

public sealed class DataContractGovernanceService(
    IAtlasOpsCapabilityRepository<DataContractGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataContractGovernanceValidator validator = new();
    private readonly DataContractGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataContractGovernanceChanged>> ExecuteAsync(
        UpdateDataContractGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataContractGovernanceChanged>.Invalid(issues);
        }

        DataContractGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataContractGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataContractGovernanceChanged>.Invalid(
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

        DataContractGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataContractGovernanceChanged>.Success(changed);
    }
}