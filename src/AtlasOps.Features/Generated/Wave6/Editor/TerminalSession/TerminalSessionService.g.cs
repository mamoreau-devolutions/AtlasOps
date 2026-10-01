namespace AtlasOps.Features.Editor.TerminalSession;

using AtlasOps.Features;

public sealed class TerminalSessionService(
    IAtlasOpsCapabilityRepository<TerminalSessionItem> repository,
    TimeProvider timeProvider)
{
    private readonly TerminalSessionValidator validator = new();
    private readonly TerminalSessionPolicy policy = new();

    public async Task<AtlasOpsOperationResult<TerminalSessionChanged>> ExecuteAsync(
        UpdateTerminalSessionCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<TerminalSessionChanged>.Invalid(issues);
        }

        TerminalSessionItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new TerminalSessionItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<TerminalSessionChanged>.Invalid(
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

        TerminalSessionChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<TerminalSessionChanged>.Success(changed);
    }
}