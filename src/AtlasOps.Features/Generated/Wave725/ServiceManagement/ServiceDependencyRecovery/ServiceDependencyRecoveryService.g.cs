namespace AtlasOps.Features.ServiceManagement.ServiceDependencyRecovery;

using AtlasOps.Features;

public sealed class ServiceDependencyRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceDependencyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceDependencyRecoveryValidator validator = new();
    private readonly ServiceDependencyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceDependencyRecoveryChanged>> ExecuteAsync(
        UpdateServiceDependencyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceDependencyRecoveryChanged>.Invalid(issues);
        }

        ServiceDependencyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceDependencyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceDependencyRecoveryChanged>.Invalid(
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

        ServiceDependencyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceDependencyRecoveryChanged>.Success(changed);
    }
}