namespace AtlasOps.Features.Identity.IdentityAuditRecovery;

using AtlasOps.Features;

public sealed class IdentityAuditRecoveryService(
    IAtlasOpsCapabilityRepository<IdentityAuditRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityAuditRecoveryValidator validator = new();
    private readonly IdentityAuditRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityAuditRecoveryChanged>> ExecuteAsync(
        UpdateIdentityAuditRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityAuditRecoveryChanged>.Invalid(issues);
        }

        IdentityAuditRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityAuditRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityAuditRecoveryChanged>.Invalid(
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

        IdentityAuditRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityAuditRecoveryChanged>.Success(changed);
    }
}