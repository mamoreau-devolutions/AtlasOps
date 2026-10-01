namespace AtlasOps.Features.Connections.ConnectionPool;

using AtlasOps.Features;

public sealed class ConnectionPoolService(
    IAtlasOpsCapabilityRepository<ConnectionPoolItem> repository,
    TimeProvider timeProvider)
{
    private readonly ConnectionPoolValidator validator = new();
    private readonly ConnectionPoolPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ConnectionPoolChanged>> ExecuteAsync(
        UpdateConnectionPoolCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ConnectionPoolChanged>.Invalid(issues);
        }

        ConnectionPoolItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ConnectionPoolItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ConnectionPoolChanged>.Invalid(
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

        ConnectionPoolChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ConnectionPoolChanged>.Success(changed);
    }
}