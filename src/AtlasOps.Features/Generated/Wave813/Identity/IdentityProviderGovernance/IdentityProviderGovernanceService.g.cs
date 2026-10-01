namespace AtlasOps.Features.Identity.IdentityProviderGovernance;

using AtlasOps.Features;

public sealed class IdentityProviderGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityProviderGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityProviderGovernanceValidator validator = new();
    private readonly IdentityProviderGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityProviderGovernanceChanged>> ExecuteAsync(
        UpdateIdentityProviderGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityProviderGovernanceChanged>.Invalid(issues);
        }

        IdentityProviderGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityProviderGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityProviderGovernanceChanged>.Invalid(
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

        IdentityProviderGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityProviderGovernanceChanged>.Success(changed);
    }
}