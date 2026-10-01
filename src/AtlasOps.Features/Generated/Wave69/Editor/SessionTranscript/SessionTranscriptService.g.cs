namespace AtlasOps.Features.Editor.SessionTranscript;

using AtlasOps.Features;

public sealed class SessionTranscriptService(
    IAtlasOpsCapabilityRepository<SessionTranscriptItem> repository,
    TimeProvider timeProvider)
{
    private readonly SessionTranscriptValidator validator = new();
    private readonly SessionTranscriptPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SessionTranscriptChanged>> ExecuteAsync(
        UpdateSessionTranscriptCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SessionTranscriptChanged>.Invalid(issues);
        }

        SessionTranscriptItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SessionTranscriptItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SessionTranscriptChanged>.Invalid(
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

        SessionTranscriptChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SessionTranscriptChanged>.Success(changed);
    }
}