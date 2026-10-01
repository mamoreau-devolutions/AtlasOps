namespace AtlasOps.Features.Architecture.ArchitectureInterfaceGovernance;

using AtlasOps.Features;

public sealed class ArchitectureInterfaceGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureInterfaceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureInterfaceGovernanceValidator validator = new();
    private readonly ArchitectureInterfaceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureInterfaceGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureInterfaceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceGovernanceChanged>.Invalid(issues);
        }

        ArchitectureInterfaceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureInterfaceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureInterfaceGovernanceChanged>.Invalid(
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

        ArchitectureInterfaceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureInterfaceGovernanceChanged>.Success(changed);
    }
}