namespace AtlasOps.Features.Connections.FileTransfer;

using AtlasOps.Features;

public sealed class FileTransferService(
    IAtlasOpsCapabilityRepository<FileTransferItem> repository,
    TimeProvider timeProvider)
{
    private readonly FileTransferValidator validator = new();
    private readonly FileTransferPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FileTransferChanged>> ExecuteAsync(
        UpdateFileTransferCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FileTransferChanged>.Invalid(issues);
        }

        FileTransferItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FileTransferItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FileTransferChanged>.Invalid(
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

        FileTransferChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FileTransferChanged>.Success(changed);
    }
}