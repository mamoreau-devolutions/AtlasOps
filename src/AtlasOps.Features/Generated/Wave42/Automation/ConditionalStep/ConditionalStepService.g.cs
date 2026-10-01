namespace AtlasOps.Features.Automation.ConditionalStep;

using AtlasOps.Features;

public sealed class ConditionalStepService(
    IAtlasOpsCapabilityRepository<ConditionalStepItem> repository,
    TimeProvider timeProvider)
{
    private readonly ConditionalStepValidator validator = new();
    private readonly ConditionalStepPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ConditionalStepChanged>> ExecuteAsync(
        UpdateConditionalStepCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ConditionalStepChanged>.Invalid(issues);
        }

        ConditionalStepItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ConditionalStepItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ConditionalStepChanged>.Invalid(
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

        ConditionalStepChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ConditionalStepChanged>.Success(changed);
    }
}