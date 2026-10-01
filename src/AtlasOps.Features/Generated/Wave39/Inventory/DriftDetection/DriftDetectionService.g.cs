namespace AtlasOps.Features.Inventory.DriftDetection;

using AtlasOps.Features;

public sealed class DriftDetectionService(
    IAtlasOpsCapabilityRepository<DriftDetectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly DriftDetectionValidator validator = new();
    private readonly DriftDetectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DriftDetectionChanged>> ExecuteAsync(
        UpdateDriftDetectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DriftDetectionChanged>.Invalid(issues);
        }

        DriftDetectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DriftDetectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DriftDetectionChanged>.Invalid(
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

        DriftDetectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DriftDetectionChanged>.Success(changed);
    }
}