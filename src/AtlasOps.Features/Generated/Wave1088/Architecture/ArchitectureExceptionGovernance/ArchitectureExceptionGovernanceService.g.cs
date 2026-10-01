namespace AtlasOps.Features.Architecture.ArchitectureExceptionGovernance;

using AtlasOps.Features;

public sealed class ArchitectureExceptionGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureExceptionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureExceptionGovernanceValidator validator = new();
    private readonly ArchitectureExceptionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureExceptionGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureExceptionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureExceptionGovernanceChanged>.Invalid(issues);
        }

        ArchitectureExceptionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureExceptionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureExceptionGovernanceChanged>.Invalid(
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

        ArchitectureExceptionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureExceptionGovernanceChanged>.Success(changed);
    }
}