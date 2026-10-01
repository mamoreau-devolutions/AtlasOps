namespace AtlasOps.Features.Sync.ConflictResolution;

using AtlasOps.Features;

public sealed class ConflictResolutionService(
    IAtlasOpsCapabilityRepository<ConflictResolutionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ConflictResolutionValidator validator = new();
    private readonly ConflictResolutionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ConflictResolutionChanged>> ExecuteAsync(
        UpdateConflictResolutionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ConflictResolutionChanged>.Invalid(issues);
        }

        ConflictResolutionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ConflictResolutionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ConflictResolutionChanged>.Invalid(
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

        ConflictResolutionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ConflictResolutionChanged>.Success(changed);
    }
}