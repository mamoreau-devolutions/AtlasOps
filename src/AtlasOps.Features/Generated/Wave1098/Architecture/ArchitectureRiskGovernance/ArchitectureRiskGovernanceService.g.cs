namespace AtlasOps.Features.Architecture.ArchitectureRiskGovernance;

using AtlasOps.Features;

public sealed class ArchitectureRiskGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureRiskGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureRiskGovernanceValidator validator = new();
    private readonly ArchitectureRiskGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureRiskGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureRiskGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureRiskGovernanceChanged>.Invalid(issues);
        }

        ArchitectureRiskGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureRiskGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureRiskGovernanceChanged>.Invalid(
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

        ArchitectureRiskGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureRiskGovernanceChanged>.Success(changed);
    }
}