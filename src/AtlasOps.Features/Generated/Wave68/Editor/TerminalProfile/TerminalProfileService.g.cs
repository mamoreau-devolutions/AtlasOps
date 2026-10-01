namespace AtlasOps.Features.Editor.TerminalProfile;

using AtlasOps.Features;

public sealed class TerminalProfileService(
    IAtlasOpsCapabilityRepository<TerminalProfileItem> repository,
    TimeProvider timeProvider)
{
    private readonly TerminalProfileValidator validator = new();
    private readonly TerminalProfilePolicy policy = new();

    public async Task<AtlasOpsOperationResult<TerminalProfileChanged>> ExecuteAsync(
        UpdateTerminalProfileCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TerminalProfileChanged>.Invalid(issues);
        }

        TerminalProfileItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TerminalProfileItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TerminalProfileChanged>.Invalid(
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

        TerminalProfileChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TerminalProfileChanged>.Success(changed);
    }
}