namespace AtlasOps.Features.Inventory.DependencyMapping;

using AtlasOps.Features;

public sealed class DependencyMappingService(
    IAtlasOpsCapabilityRepository<DependencyMappingItem> repository,
    TimeProvider timeProvider)
{
    private readonly DependencyMappingValidator validator = new();
    private readonly DependencyMappingPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DependencyMappingChanged>> ExecuteAsync(
        UpdateDependencyMappingCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DependencyMappingChanged>.Invalid(issues);
        }

        DependencyMappingItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DependencyMappingItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DependencyMappingChanged>.Invalid(
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

        DependencyMappingChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DependencyMappingChanged>.Success(changed);
    }
}