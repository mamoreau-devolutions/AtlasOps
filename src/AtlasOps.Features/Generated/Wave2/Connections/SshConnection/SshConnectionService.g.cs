namespace AtlasOps.Features.Connections.SshConnection;

using AtlasOps.Features;

public sealed class SshConnectionService(
    IAtlasOpsCapabilityRepository<SshConnectionItem> repository,
    TimeProvider timeProvider)
{
    private readonly SshConnectionValidator validator = new();
    private readonly SshConnectionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SshConnectionChanged>> ExecuteAsync(
        UpdateSshConnectionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SshConnectionChanged>.Invalid(issues);
        }

        SshConnectionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SshConnectionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SshConnectionChanged>.Invalid(
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

        SshConnectionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SshConnectionChanged>.Success(changed);
    }
}