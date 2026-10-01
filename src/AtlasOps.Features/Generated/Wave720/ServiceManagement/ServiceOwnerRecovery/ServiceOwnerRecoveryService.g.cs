namespace AtlasOps.Features.ServiceManagement.ServiceOwnerRecovery;

using AtlasOps.Features;

public sealed class ServiceOwnerRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceOwnerRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceOwnerRecoveryValidator validator = new();
    private readonly ServiceOwnerRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceOwnerRecoveryChanged>> ExecuteAsync(
        UpdateServiceOwnerRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceOwnerRecoveryChanged>.Invalid(issues);
        }

        ServiceOwnerRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceOwnerRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceOwnerRecoveryChanged>.Invalid(
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

        ServiceOwnerRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceOwnerRecoveryChanged>.Success(changed);
    }
}