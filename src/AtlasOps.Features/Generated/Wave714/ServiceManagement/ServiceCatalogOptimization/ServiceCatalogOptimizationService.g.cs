namespace AtlasOps.Features.ServiceManagement.ServiceCatalogOptimization;

using AtlasOps.Features;

public sealed class ServiceCatalogOptimizationService(
    IAtlasOpsCapabilityRepository<ServiceCatalogOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceCatalogOptimizationValidator validator = new();
    private readonly ServiceCatalogOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceCatalogOptimizationChanged>> ExecuteAsync(
        UpdateServiceCatalogOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceCatalogOptimizationChanged>.Invalid(issues);
        }

        ServiceCatalogOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceCatalogOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceCatalogOptimizationChanged>.Invalid(
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

        ServiceCatalogOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceCatalogOptimizationChanged>.Success(changed);
    }
}