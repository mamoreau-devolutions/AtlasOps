namespace AtlasOps.Features.Identity.IdentityClaimGovernance;

using AtlasOps.Features;

public sealed class IdentityClaimGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityClaimGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityClaimGovernanceValidator validator = new();
    private readonly IdentityClaimGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityClaimGovernanceChanged>> ExecuteAsync(
        UpdateIdentityClaimGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityClaimGovernanceChanged>.Invalid(issues);
        }

        IdentityClaimGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityClaimGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityClaimGovernanceChanged>.Invalid(
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

        IdentityClaimGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityClaimGovernanceChanged>.Success(changed);
    }
}