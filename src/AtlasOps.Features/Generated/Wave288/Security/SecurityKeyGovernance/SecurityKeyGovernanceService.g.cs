namespace AtlasOps.Features.Security.SecurityKeyGovernance;

using AtlasOps.Features;

public sealed class SecurityKeyGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityKeyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityKeyGovernanceValidator validator = new();
    private readonly SecurityKeyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityKeyGovernanceChanged>> ExecuteAsync(
        UpdateSecurityKeyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityKeyGovernanceChanged>.Invalid(issues);
        }

        SecurityKeyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityKeyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityKeyGovernanceChanged>.Invalid(
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

        SecurityKeyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityKeyGovernanceChanged>.Success(changed);
    }
}