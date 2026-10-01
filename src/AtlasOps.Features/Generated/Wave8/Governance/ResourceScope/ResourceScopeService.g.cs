namespace AtlasOps.Features.Governance.ResourceScope;

using AtlasOps.Features;

public sealed class ResourceScopeService(
    IAtlasOpsCapabilityRepository<ResourceScopeItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResourceScopeValidator validator = new();
    private readonly ResourceScopePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResourceScopeChanged>> ExecuteAsync(
        UpdateResourceScopeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResourceScopeChanged>.Invalid(issues);
        }

        ResourceScopeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResourceScopeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResourceScopeChanged>.Invalid(
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

        ResourceScopeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResourceScopeChanged>.Success(changed);
    }
}