namespace AtlasOps.Features.Sync.ChangeVector;

using AtlasOps.Features;

public sealed class ChangeVectorService(
    IAtlasOpsCapabilityRepository<ChangeVectorItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChangeVectorValidator validator = new();
    private readonly ChangeVectorPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChangeVectorChanged>> ExecuteAsync(
        UpdateChangeVectorCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChangeVectorChanged>.Invalid(issues);
        }

        ChangeVectorItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChangeVectorItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChangeVectorChanged>.Invalid(
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

        ChangeVectorChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChangeVectorChanged>.Success(changed);
    }
}