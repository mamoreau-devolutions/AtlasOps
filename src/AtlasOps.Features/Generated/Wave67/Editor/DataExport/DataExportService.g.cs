namespace AtlasOps.Features.Editor.DataExport;

using AtlasOps.Features;

public sealed class DataExportService(
    IAtlasOpsCapabilityRepository<DataExportItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataExportValidator validator = new();
    private readonly DataExportPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataExportChanged>> ExecuteAsync(
        UpdateDataExportCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataExportChanged>.Invalid(issues);
        }

        DataExportItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataExportItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataExportChanged>.Invalid(
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

        DataExportChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataExportChanged>.Success(changed);
    }
}