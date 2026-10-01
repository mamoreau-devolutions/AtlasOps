namespace AtlasOps.Features.Connections.LocalProcess;

using AtlasOps.Features;

public sealed class LocalProcessService(
    IAtlasOpsCapabilityRepository<LocalProcessItem> repository,
    TimeProvider timeProvider)
{
    private readonly LocalProcessValidator validator = new();
    private readonly LocalProcessPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LocalProcessChanged>> ExecuteAsync(
        UpdateLocalProcessCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LocalProcessChanged>.Invalid(issues);
        }

        LocalProcessItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LocalProcessItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LocalProcessChanged>.Invalid(
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

        LocalProcessChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LocalProcessChanged>.Success(changed);
    }
}