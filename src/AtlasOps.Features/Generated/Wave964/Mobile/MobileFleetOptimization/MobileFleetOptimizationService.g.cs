namespace AtlasOps.Features.Mobile.MobileFleetOptimization;

using AtlasOps.Features;

public sealed class MobileFleetOptimizationService(
    IAtlasOpsCapabilityRepository<MobileFleetOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileFleetOptimizationValidator validator = new();
    private readonly MobileFleetOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileFleetOptimizationChanged>> ExecuteAsync(
        UpdateMobileFleetOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileFleetOptimizationChanged>.Invalid(issues);
        }

        MobileFleetOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileFleetOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileFleetOptimizationChanged>.Invalid(
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

        MobileFleetOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileFleetOptimizationChanged>.Success(changed);
    }
}