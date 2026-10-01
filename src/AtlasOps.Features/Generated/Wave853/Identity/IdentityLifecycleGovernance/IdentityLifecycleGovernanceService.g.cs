namespace AtlasOps.Features.Identity.IdentityLifecycleGovernance;

using AtlasOps.Features;

public sealed class IdentityLifecycleGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityLifecycleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityLifecycleGovernanceValidator validator = new();
    private readonly IdentityLifecycleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityLifecycleGovernanceChanged>> ExecuteAsync(
        UpdateIdentityLifecycleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityLifecycleGovernanceChanged>.Invalid(issues);
        }

        IdentityLifecycleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityLifecycleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityLifecycleGovernanceChanged>.Invalid(
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

        IdentityLifecycleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityLifecycleGovernanceChanged>.Success(changed);
    }
}