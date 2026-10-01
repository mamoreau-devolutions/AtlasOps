namespace AtlasOps.Features.ServiceManagement.ServiceRequestOptimization;

using AtlasOps.Features;

public sealed class ServiceRequestOptimizationService(
    IAtlasOpsCapabilityRepository<ServiceRequestOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceRequestOptimizationValidator validator = new();
    private readonly ServiceRequestOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceRequestOptimizationChanged>> ExecuteAsync(
        UpdateServiceRequestOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceRequestOptimizationChanged>.Invalid(issues);
        }

        ServiceRequestOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceRequestOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceRequestOptimizationChanged>.Invalid(
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

        ServiceRequestOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceRequestOptimizationChanged>.Success(changed);
    }
}