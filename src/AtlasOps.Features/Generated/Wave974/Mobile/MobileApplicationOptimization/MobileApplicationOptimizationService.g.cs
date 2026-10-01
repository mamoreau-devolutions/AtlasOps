namespace AtlasOps.Features.Mobile.MobileApplicationOptimization;

using AtlasOps.Features;

public sealed class MobileApplicationOptimizationService(
    IAtlasOpsCapabilityRepository<MobileApplicationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileApplicationOptimizationValidator validator = new();
    private readonly MobileApplicationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileApplicationOptimizationChanged>> ExecuteAsync(
        UpdateMobileApplicationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileApplicationOptimizationChanged>.Invalid(issues);
        }

        MobileApplicationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileApplicationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileApplicationOptimizationChanged>.Invalid(
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

        MobileApplicationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileApplicationOptimizationChanged>.Success(changed);
    }
}