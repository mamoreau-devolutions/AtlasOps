namespace AtlasOps.Features.ServiceManagement.ServiceOwnerOptimization;

using AtlasOps.Features;

public sealed class ServiceOwnerOptimizationService(
    IAtlasOpsCapabilityRepository<ServiceOwnerOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceOwnerOptimizationValidator validator = new();
    private readonly ServiceOwnerOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceOwnerOptimizationChanged>> ExecuteAsync(
        UpdateServiceOwnerOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceOwnerOptimizationChanged>.Invalid(issues);
        }

        ServiceOwnerOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceOwnerOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceOwnerOptimizationChanged>.Invalid(
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

        ServiceOwnerOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceOwnerOptimizationChanged>.Success(changed);
    }
}