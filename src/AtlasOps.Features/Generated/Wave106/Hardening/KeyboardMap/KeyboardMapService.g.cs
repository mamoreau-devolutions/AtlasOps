namespace AtlasOps.Features.Hardening.KeyboardMap;

using AtlasOps.Features;

public sealed class KeyboardMapService(
    IAtlasOpsCapabilityRepository<KeyboardMapItem> repository,
    TimeProvider timeProvider)
{
    private readonly KeyboardMapValidator validator = new();
    private readonly KeyboardMapPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KeyboardMapChanged>> ExecuteAsync(
        UpdateKeyboardMapCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KeyboardMapChanged>.Invalid(issues);
        }

        KeyboardMapItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KeyboardMapItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KeyboardMapChanged>.Invalid(
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

        KeyboardMapChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KeyboardMapChanged>.Success(changed);
    }
}