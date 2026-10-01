namespace AtlasOps.Features.Security.SecuritySessionGovernance;

using AtlasOps.Features;

public sealed class SecuritySessionGovernanceService(
    IAtlasOpsCapabilityRepository<SecuritySessionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecuritySessionGovernanceValidator validator = new();
    private readonly SecuritySessionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecuritySessionGovernanceChanged>> ExecuteAsync(
        UpdateSecuritySessionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecuritySessionGovernanceChanged>.Invalid(issues);
        }

        SecuritySessionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecuritySessionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecuritySessionGovernanceChanged>.Invalid(
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

        SecuritySessionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecuritySessionGovernanceChanged>.Success(changed);
    }
}