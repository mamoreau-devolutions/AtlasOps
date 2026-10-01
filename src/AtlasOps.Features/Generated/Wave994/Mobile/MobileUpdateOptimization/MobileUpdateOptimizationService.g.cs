namespace AtlasOps.Features.Mobile.MobileUpdateOptimization;

using AtlasOps.Features;

public sealed class MobileUpdateOptimizationService(
    IAtlasOpsCapabilityRepository<MobileUpdateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileUpdateOptimizationValidator validator = new();
    private readonly MobileUpdateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileUpdateOptimizationChanged>> ExecuteAsync(
        UpdateMobileUpdateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileUpdateOptimizationChanged>.Invalid(issues);
        }

        MobileUpdateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileUpdateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileUpdateOptimizationChanged>.Invalid(
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

        MobileUpdateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileUpdateOptimizationChanged>.Success(changed);
    }
}