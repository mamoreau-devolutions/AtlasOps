namespace AtlasOps.Features.Automation.ChangeWindow;

using AtlasOps.Features;

public sealed class ChangeWindowService(
    IAtlasOpsCapabilityRepository<ChangeWindowItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChangeWindowValidator validator = new();
    private readonly ChangeWindowPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChangeWindowChanged>> ExecuteAsync(
        UpdateChangeWindowCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChangeWindowChanged>.Invalid(issues);
        }

        ChangeWindowItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChangeWindowItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChangeWindowChanged>.Invalid(
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

        ChangeWindowChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChangeWindowChanged>.Success(changed);
    }
}