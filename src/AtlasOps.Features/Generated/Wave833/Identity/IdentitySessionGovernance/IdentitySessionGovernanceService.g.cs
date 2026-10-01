namespace AtlasOps.Features.Identity.IdentitySessionGovernance;

using AtlasOps.Features;

public sealed class IdentitySessionGovernanceService(
    IAtlasOpsCapabilityRepository<IdentitySessionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentitySessionGovernanceValidator validator = new();
    private readonly IdentitySessionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentitySessionGovernanceChanged>> ExecuteAsync(
        UpdateIdentitySessionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentitySessionGovernanceChanged>.Invalid(issues);
        }

        IdentitySessionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentitySessionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentitySessionGovernanceChanged>.Invalid(
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

        IdentitySessionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentitySessionGovernanceChanged>.Success(changed);
    }
}