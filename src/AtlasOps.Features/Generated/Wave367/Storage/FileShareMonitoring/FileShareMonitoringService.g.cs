namespace AtlasOps.Features.Storage.FileShareMonitoring;

using AtlasOps.Features;

public sealed class FileShareMonitoringService(
    IAtlasOpsCapabilityRepository<FileShareMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly FileShareMonitoringValidator validator = new();
    private readonly FileShareMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<FileShareMonitoringChanged>> ExecuteAsync(
        UpdateFileShareMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FileShareMonitoringChanged>.Invalid(issues);
        }

        FileShareMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FileShareMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FileShareMonitoringChanged>.Invalid(
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

        FileShareMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FileShareMonitoringChanged>.Success(changed);
    }
}