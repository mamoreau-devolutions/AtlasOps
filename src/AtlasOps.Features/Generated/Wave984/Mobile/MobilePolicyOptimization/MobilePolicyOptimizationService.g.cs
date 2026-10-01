namespace AtlasOps.Features.Mobile.MobilePolicyOptimization;

using AtlasOps.Features;

public sealed class MobilePolicyOptimizationService(
    IAtlasOpsCapabilityRepository<MobilePolicyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobilePolicyOptimizationValidator validator = new();
    private readonly MobilePolicyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobilePolicyOptimizationChanged>> ExecuteAsync(
        UpdateMobilePolicyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobilePolicyOptimizationChanged>.Invalid(issues);
        }

        MobilePolicyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobilePolicyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobilePolicyOptimizationChanged>.Invalid(
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

        MobilePolicyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobilePolicyOptimizationChanged>.Success(changed);
    }
}