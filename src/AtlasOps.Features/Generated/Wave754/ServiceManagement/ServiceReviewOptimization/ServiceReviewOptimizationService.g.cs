namespace AtlasOps.Features.ServiceManagement.ServiceReviewOptimization;

using AtlasOps.Features;

public sealed class ServiceReviewOptimizationService(
    IAtlasOpsCapabilityRepository<ServiceReviewOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceReviewOptimizationValidator validator = new();
    private readonly ServiceReviewOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceReviewOptimizationChanged>> ExecuteAsync(
        UpdateServiceReviewOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceReviewOptimizationChanged>.Invalid(issues);
        }

        ServiceReviewOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceReviewOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceReviewOptimizationChanged>.Invalid(
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

        ServiceReviewOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceReviewOptimizationChanged>.Success(changed);
    }
}