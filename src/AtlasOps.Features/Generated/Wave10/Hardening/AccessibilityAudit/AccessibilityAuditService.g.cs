namespace AtlasOps.Features.Hardening.AccessibilityAudit;

using AtlasOps.Features;

public sealed class AccessibilityAuditService(
    IAtlasOpsCapabilityRepository<AccessibilityAuditItem> repository,
    TimeProvider timeProvider)
{
    private readonly AccessibilityAuditValidator validator = new();
    private readonly AccessibilityAuditPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AccessibilityAuditChanged>> ExecuteAsync(
        UpdateAccessibilityAuditCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AccessibilityAuditChanged>.Invalid(issues);
        }

        AccessibilityAuditItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AccessibilityAuditItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AccessibilityAuditChanged>.Invalid(
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

        AccessibilityAuditChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AccessibilityAuditChanged>.Success(changed);
    }
}