namespace AtlasOps.Features.Cloud.AzureSubscriptionOptimization;

using AtlasOps.Features;

public sealed class AzureSubscriptionOptimizationService(
    IAtlasOpsCapabilityRepository<AzureSubscriptionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly AzureSubscriptionOptimizationValidator validator = new();
    private readonly AzureSubscriptionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AzureSubscriptionOptimizationChanged>> ExecuteAsync(
        UpdateAzureSubscriptionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AzureSubscriptionOptimizationChanged>.Invalid(issues);
        }

        AzureSubscriptionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AzureSubscriptionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AzureSubscriptionOptimizationChanged>.Invalid(
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

        AzureSubscriptionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AzureSubscriptionOptimizationChanged>.Success(changed);
    }
}