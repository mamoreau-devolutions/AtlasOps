namespace AtlasOps.Features.Identity.IdentityApplicationGovernance;

using AtlasOps.Features;

public sealed class IdentityApplicationGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityApplicationGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityApplicationGovernanceValidator validator = new();
    private readonly IdentityApplicationGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityApplicationGovernanceChanged>> ExecuteAsync(
        UpdateIdentityApplicationGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityApplicationGovernanceChanged>.Invalid(issues);
        }

        IdentityApplicationGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityApplicationGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityApplicationGovernanceChanged>.Invalid(
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

        IdentityApplicationGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityApplicationGovernanceChanged>.Success(changed);
    }
}