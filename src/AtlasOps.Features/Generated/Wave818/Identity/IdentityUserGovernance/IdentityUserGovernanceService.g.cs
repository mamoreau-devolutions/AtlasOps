namespace AtlasOps.Features.Identity.IdentityUserGovernance;

using AtlasOps.Features;

public sealed class IdentityUserGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityUserGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityUserGovernanceValidator validator = new();
    private readonly IdentityUserGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityUserGovernanceChanged>> ExecuteAsync(
        UpdateIdentityUserGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityUserGovernanceChanged>.Invalid(issues);
        }

        IdentityUserGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityUserGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityUserGovernanceChanged>.Invalid(
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

        IdentityUserGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityUserGovernanceChanged>.Success(changed);
    }
}