namespace AtlasOps.Features.Connections.RdpConnection;

using AtlasOps.Features;

public sealed class RdpConnectionService(
    IAtlasOpsCapabilityRepository<RdpConnectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly RdpConnectionValidator validator = new();
    private readonly RdpConnectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RdpConnectionChanged>> ExecuteAsync(
        UpdateRdpConnectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RdpConnectionChanged>.Invalid(issues);
        }

        RdpConnectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RdpConnectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RdpConnectionChanged>.Invalid(
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

        RdpConnectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RdpConnectionChanged>.Success(changed);
    }
}