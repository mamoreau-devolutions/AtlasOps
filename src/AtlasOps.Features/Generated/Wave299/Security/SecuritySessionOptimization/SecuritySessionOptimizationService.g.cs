namespace AtlasOps.Features.Security.SecuritySessionOptimization;

using AtlasOps.Features;

public sealed class SecuritySessionOptimizationService(
    IAtlasOpsCapabilityRepository<SecuritySessionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecuritySessionOptimizationValidator validator = new();
    private readonly SecuritySessionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecuritySessionOptimizationChanged>> ExecuteAsync(
        UpdateSecuritySessionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecuritySessionOptimizationChanged>.Invalid(issues);
        }

        SecuritySessionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecuritySessionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecuritySessionOptimizationChanged>.Invalid(
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

        SecuritySessionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecuritySessionOptimizationChanged>.Success(changed);
    }
}