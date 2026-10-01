namespace AtlasOps.Features.Sync.DataArchive;

using AtlasOps.Features;

public sealed class DataArchiveService(
    IAtlasOpsCapabilityRepository<DataArchiveItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataArchiveValidator validator = new();
    private readonly DataArchivePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataArchiveChanged>> ExecuteAsync(
        UpdateDataArchiveCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataArchiveChanged>.Invalid(issues);
        }

        DataArchiveItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataArchiveItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataArchiveChanged>.Invalid(
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

        DataArchiveChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataArchiveChanged>.Success(changed);
    }
}