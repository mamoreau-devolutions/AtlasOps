namespace AtlasOps.Features.Security.SecurityKeyOptimization;

using AtlasOps.Features;

public sealed class SecurityKeyOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityKeyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityKeyOptimizationValidator validator = new();
    private readonly SecurityKeyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityKeyOptimizationChanged>> ExecuteAsync(
        UpdateSecurityKeyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityKeyOptimizationChanged>.Invalid(issues);
        }

        SecurityKeyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityKeyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityKeyOptimizationChanged>.Invalid(
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

        SecurityKeyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityKeyOptimizationChanged>.Success(changed);
    }
}