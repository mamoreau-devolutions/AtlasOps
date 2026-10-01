namespace AtlasOps.Features.Storage.FileShareRecovery;

using AtlasOps.Features;

public sealed class FileShareRecoveryService(
    IAtlasOpsCapabilityRepository<FileShareRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly FileShareRecoveryValidator validator = new();
    private readonly FileShareRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FileShareRecoveryChanged>> ExecuteAsync(
        UpdateFileShareRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FileShareRecoveryChanged>.Invalid(issues);
        }

        FileShareRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FileShareRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FileShareRecoveryChanged>.Invalid(
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

        FileShareRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FileShareRecoveryChanged>.Success(changed);
    }
}