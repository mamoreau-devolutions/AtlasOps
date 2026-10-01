namespace AtlasOps.Features.Identity.IdentityAuditGovernance;

using AtlasOps.Features;

public sealed class IdentityAuditGovernanceService(
    IAtlasOpsCapabilityRepository<IdentityAuditGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityAuditGovernanceValidator validator = new();
    private readonly IdentityAuditGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityAuditGovernanceChanged>> ExecuteAsync(
        UpdateIdentityAuditGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityAuditGovernanceChanged>.Invalid(issues);
        }

        IdentityAuditGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityAuditGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityAuditGovernanceChanged>.Invalid(
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

        IdentityAuditGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityAuditGovernanceChanged>.Success(changed);
    }
}