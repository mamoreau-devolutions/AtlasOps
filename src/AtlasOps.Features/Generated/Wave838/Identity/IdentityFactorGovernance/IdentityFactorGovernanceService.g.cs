namespace AtlasOps.Features.Identity.IdentityFactorGovernance;

using AtlasOps.Features;

public sealed class IdentityFactorGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityFactorGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityFactorGovernanceValidator validator = new();
    private readonly IdentityFactorGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityFactorGovernanceChanged>> ExecuteAsync(
        UpdateIdentityFactorGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityFactorGovernanceChanged>.Invalid(issues);
        }

        IdentityFactorGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityFactorGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityFactorGovernanceChanged>.Invalid(
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

        IdentityFactorGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityFactorGovernanceChanged>.Success(changed);
    }
}