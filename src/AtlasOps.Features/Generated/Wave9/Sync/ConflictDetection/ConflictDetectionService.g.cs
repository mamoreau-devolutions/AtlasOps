namespace AtlasOps.Features.Sync.ConflictDetection;

using AtlasOps.Features;

public sealed class ConflictDetectionService(
    IAtlasOpsCapabilityRepository<ConflictDetectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly ConflictDetectionValidator validator = new();
    private readonly ConflictDetectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ConflictDetectionChanged>> ExecuteAsync(
        UpdateConflictDetectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ConflictDetectionChanged>.Invalid(issues);
        }

        ConflictDetectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ConflictDetectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ConflictDetectionChanged>.Invalid(
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

        ConflictDetectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ConflictDetectionChanged>.Success(changed);
    }
}