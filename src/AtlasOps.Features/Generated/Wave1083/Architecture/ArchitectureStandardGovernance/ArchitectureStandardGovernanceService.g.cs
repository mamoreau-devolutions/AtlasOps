namespace AtlasOps.Features.Architecture.ArchitectureStandardGovernance;

using AtlasOps.Features;

public sealed class ArchitectureStandardGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureStandardGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureStandardGovernanceValidator validator = new();
    private readonly ArchitectureStandardGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureStandardGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureStandardGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureStandardGovernanceChanged>.Invalid(issues);
        }

        ArchitectureStandardGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureStandardGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureStandardGovernanceChanged>.Invalid(
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

        ArchitectureStandardGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureStandardGovernanceChanged>.Success(changed);
    }
}