namespace AtlasOps.Features.Platform.CacheCoordination;

using AtlasOps.Features;

public sealed class CacheCoordinationService(
    IAtlasOpsCapabilityRepository<CacheCoordinationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CacheCoordinationValidator validator = new();
    private readonly CacheCoordinationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CacheCoordinationChanged>> ExecuteAsync(
        UpdateCacheCoordinationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CacheCoordinationChanged>.Invalid(issues);
        }

        CacheCoordinationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CacheCoordinationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CacheCoordinationChanged>.Invalid(
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

        CacheCoordinationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CacheCoordinationChanged>.Success(changed);
    }
}