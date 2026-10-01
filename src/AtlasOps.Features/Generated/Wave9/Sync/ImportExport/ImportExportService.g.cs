namespace AtlasOps.Features.Sync.ImportExport;

using AtlasOps.Features;

public sealed class ImportExportService(
    IAtlasOpsCapabilityRepository<ImportExportItem> repository,
    TimeProvider timeProvider)
{
    private readonly ImportExportValidator validator = new();
    private readonly ImportExportPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ImportExportChanged>> ExecuteAsync(
        UpdateImportExportCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ImportExportChanged>.Invalid(issues);
        }

        ImportExportItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ImportExportItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ImportExportChanged>.Invalid(
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

        ImportExportChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ImportExportChanged>.Success(changed);
    }
}