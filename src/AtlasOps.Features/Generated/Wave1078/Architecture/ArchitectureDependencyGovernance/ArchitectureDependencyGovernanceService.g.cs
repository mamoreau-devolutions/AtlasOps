namespace AtlasOps.Features.Architecture.ArchitectureDependencyGovernance;

using AtlasOps.Features;

public sealed class ArchitectureDependencyGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureDependencyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDependencyGovernanceValidator validator = new();
    private readonly ArchitectureDependencyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDependencyGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureDependencyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDependencyGovernanceChanged>.Invalid(issues);
        }

        ArchitectureDependencyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDependencyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDependencyGovernanceChanged>.Invalid(
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

        ArchitectureDependencyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDependencyGovernanceChanged>.Success(changed);
    }
}