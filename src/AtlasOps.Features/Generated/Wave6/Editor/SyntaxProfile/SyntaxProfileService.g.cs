namespace AtlasOps.Features.Editor.SyntaxProfile;

using AtlasOps.Features;

public sealed class SyntaxProfileService(
    IAtlasOpsCapabilityRepository<SyntaxProfileItem> repository,
    TimeProvider timeProvider)
{
    private readonly SyntaxProfileValidator validator = new();
    private readonly SyntaxProfilePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SyntaxProfileChanged>> ExecuteAsync(
        UpdateSyntaxProfileCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SyntaxProfileChanged>.Invalid(issues);
        }

        SyntaxProfileItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SyntaxProfileItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SyntaxProfileChanged>.Invalid(
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

        SyntaxProfileChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SyntaxProfileChanged>.Success(changed);
    }
}