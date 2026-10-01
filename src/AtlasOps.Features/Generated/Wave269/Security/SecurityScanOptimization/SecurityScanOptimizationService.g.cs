namespace AtlasOps.Features.Security.SecurityScanOptimization;

using AtlasOps.Features;

public sealed class SecurityScanOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityScanOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityScanOptimizationValidator validator = new();
    private readonly SecurityScanOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityScanOptimizationChanged>> ExecuteAsync(
        UpdateSecurityScanOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityScanOptimizationChanged>.Invalid(issues);
        }

        SecurityScanOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityScanOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityScanOptimizationChanged>.Invalid(
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

        SecurityScanOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityScanOptimizationChanged>.Success(changed);
    }
}