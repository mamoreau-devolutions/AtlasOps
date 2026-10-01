namespace AtlasOps.Features.Architecture.ArchitectureDecisionGovernance;

using AtlasOps.Features;

public sealed class ArchitectureDecisionGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureDecisionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDecisionGovernanceValidator validator = new();
    private readonly ArchitectureDecisionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDecisionGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureDecisionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDecisionGovernanceChanged>.Invalid(issues);
        }

        ArchitectureDecisionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDecisionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDecisionGovernanceChanged>.Invalid(
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

        ArchitectureDecisionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDecisionGovernanceChanged>.Success(changed);
    }
}