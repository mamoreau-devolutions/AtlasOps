namespace AtlasOps.Features.Incidents.RotationHandoff;

using AtlasOps.Features;

public sealed class RotationHandoffService(
    IAtlasOpsCapabilityRepository<RotationHandoffItem> repository,
    TimeProvider timeProvider)
{
    private readonly RotationHandoffValidator validator = new();
    private readonly RotationHandoffPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RotationHandoffChanged>> ExecuteAsync(
        UpdateRotationHandoffCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RotationHandoffChanged>.Invalid(issues);
        }

        RotationHandoffItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RotationHandoffItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RotationHandoffChanged>.Invalid(
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

        RotationHandoffChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RotationHandoffChanged>.Success(changed);
    }
}