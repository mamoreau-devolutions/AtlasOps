namespace AtlasOps.Features.Mobile.MobileDeviceOptimization;

using AtlasOps.Features;

public sealed class MobileDeviceOptimizationService(
    IAtlasOpsCapabilityRepository<MobileDeviceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileDeviceOptimizationValidator validator = new();
    private readonly MobileDeviceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileDeviceOptimizationChanged>> ExecuteAsync(
        UpdateMobileDeviceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileDeviceOptimizationChanged>.Invalid(issues);
        }

        MobileDeviceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileDeviceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileDeviceOptimizationChanged>.Invalid(
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

        MobileDeviceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileDeviceOptimizationChanged>.Success(changed);
    }
}