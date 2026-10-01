namespace AtlasOps.Features.Connections.ConnectionImport;

using AtlasOps.Features;

public sealed class ConnectionImportService(
    IAtlasOpsCapabilityRepository<ConnectionImportItem> repository,
    TimeProvider timeProvider)
{
    private readonly ConnectionImportValidator validator = new();
    private readonly ConnectionImportPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ConnectionImportChanged>> ExecuteAsync(
        UpdateConnectionImportCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ConnectionImportChanged>.Invalid(issues);
        }

        ConnectionImportItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ConnectionImportItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ConnectionImportChanged>.Invalid(
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

        ConnectionImportChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ConnectionImportChanged>.Success(changed);
    }
}