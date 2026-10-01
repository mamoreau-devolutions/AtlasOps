namespace AtlasOps.Features.Desktop.DesktopPolicyOptimization;

using AtlasOps.Features;

public sealed class DesktopPolicyOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopPolicyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPolicyOptimizationValidator validator = new();
    private readonly DesktopPolicyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPolicyOptimizationChanged>> ExecuteAsync(
        UpdateDesktopPolicyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPolicyOptimizationChanged>.Invalid(issues);
        }

        DesktopPolicyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPolicyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPolicyOptimizationChanged>.Invalid(
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

        DesktopPolicyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPolicyOptimizationChanged>.Success(changed);
    }
}