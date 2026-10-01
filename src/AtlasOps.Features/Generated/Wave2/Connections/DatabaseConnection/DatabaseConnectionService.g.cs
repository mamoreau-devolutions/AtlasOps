namespace AtlasOps.Features.Connections.DatabaseConnection;

using AtlasOps.Features;

public sealed class DatabaseConnectionService(
    IAtlasOpsCapabilityRepository<DatabaseConnectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseConnectionValidator validator = new();
    private readonly DatabaseConnectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseConnectionChanged>> ExecuteAsync(
        UpdateDatabaseConnectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseConnectionChanged>.Invalid(issues);
        }

        DatabaseConnectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseConnectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseConnectionChanged>.Invalid(
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

        DatabaseConnectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseConnectionChanged>.Success(changed);
    }
}