namespace AtlasOps.Features.Governance.ActorIdentity;

using AtlasOps.Features;

public sealed class ActorIdentityService(
    IAtlasOpsCapabilityRepository<ActorIdentityItem> repository,
    TimeProvider timeProvider)
{
    private readonly ActorIdentityValidator validator = new();
    private readonly ActorIdentityPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ActorIdentityChanged>> ExecuteAsync(
        UpdateActorIdentityCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ActorIdentityChanged>.Invalid(issues);
        }

        ActorIdentityItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ActorIdentityItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ActorIdentityChanged>.Invalid(
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

        ActorIdentityChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ActorIdentityChanged>.Success(changed);
    }
}