namespace AtlasOps.Features.ServiceManagement.ServiceCatalogRecovery;

using AtlasOps.Features;

public sealed class ServiceCatalogRecoveryService(
    IAtlasOpsCapabilityRepository<ServiceCatalogRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceCatalogRecoveryValidator validator = new();
    private readonly ServiceCatalogRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceCatalogRecoveryChanged>> ExecuteAsync(
        UpdateServiceCatalogRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceCatalogRecoveryChanged>.Invalid(issues);
        }

        ServiceCatalogRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceCatalogRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceCatalogRecoveryChanged>.Invalid(
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

        ServiceCatalogRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceCatalogRecoveryChanged>.Success(changed);
    }
}