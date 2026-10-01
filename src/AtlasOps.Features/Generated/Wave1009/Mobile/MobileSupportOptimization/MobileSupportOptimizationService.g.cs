namespace AtlasOps.Features.Mobile.MobileSupportOptimization;

using AtlasOps.Features;

public sealed class MobileSupportOptimizationService(
    IAtlasOpsCapabilityRepository<MobileSupportOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileSupportOptimizationValidator validator = new();
    private readonly MobileSupportOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileSupportOptimizationChanged>> ExecuteAsync(
        UpdateMobileSupportOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileSupportOptimizationChanged>.Invalid(issues);
        }

        MobileSupportOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileSupportOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileSupportOptimizationChanged>.Invalid(
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

        MobileSupportOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileSupportOptimizationChanged>.Success(changed);
    }
}