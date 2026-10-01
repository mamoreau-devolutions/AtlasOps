namespace AtlasOps.Features.ServiceManagement.ServiceRequestRecovery;

using AtlasOps.Features;

public sealed class ServiceRequestRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceRequestRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceRequestRecoveryValidator validator = new();
    private readonly ServiceRequestRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceRequestRecoveryChanged>> ExecuteAsync(
        UpdateServiceRequestRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceRequestRecoveryChanged>.Invalid(issues);
        }

        ServiceRequestRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceRequestRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceRequestRecoveryChanged>.Invalid(
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

        ServiceRequestRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceRequestRecoveryChanged>.Success(changed);
    }
}