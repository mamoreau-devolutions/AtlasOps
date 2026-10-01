namespace AtlasOps.Features.Security.SecurityBoundaryGovernance;

using AtlasOps.Features;

public sealed class SecurityBoundaryGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityBoundaryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBoundaryGovernanceValidator validator = new();
    private readonly SecurityBoundaryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBoundaryGovernanceChanged>> ExecuteAsync(
        UpdateSecurityBoundaryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBoundaryGovernanceChanged>.Invalid(issues);
        }

        SecurityBoundaryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBoundaryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBoundaryGovernanceChanged>.Invalid(
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

        SecurityBoundaryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBoundaryGovernanceChanged>.Success(changed);
    }
}