namespace AtlasOps.Features.Identity.IdentityAuditOptimization;

using AtlasOps.Features;

public sealed class IdentityAuditOptimizationService(
    IAtlasOpsCapabilityRepository<IdentityAuditOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityAuditOptimizationValidator validator = new();
    private readonly IdentityAuditOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityAuditOptimizationChanged>> ExecuteAsync(
        UpdateIdentityAuditOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityAuditOptimizationChanged>.Invalid(issues);
        }

        IdentityAuditOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityAuditOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityAuditOptimizationChanged>.Invalid(
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

        IdentityAuditOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityAuditOptimizationChanged>.Success(changed);
    }
}