namespace AtlasOps.Features.ServiceManagement.ServiceDependencyOptimization;

using AtlasOps.Features;

public sealed class ServiceDependencyOptimizationService(
    IAtlasOpsCapabilityRepository<ServiceDependencyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceDependencyOptimizationValidator validator = new();
    private readonly ServiceDependencyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceDependencyOptimizationChanged>> ExecuteAsync(
        UpdateServiceDependencyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceDependencyOptimizationChanged>.Invalid(issues);
        }

        ServiceDependencyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceDependencyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceDependencyOptimizationChanged>.Invalid(
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

        ServiceDependencyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceDependencyOptimizationChanged>.Success(changed);
    }
}