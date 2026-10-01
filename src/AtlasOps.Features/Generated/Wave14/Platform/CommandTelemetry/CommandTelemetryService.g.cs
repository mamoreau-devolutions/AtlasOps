namespace AtlasOps.Features.Platform.CommandTelemetry;

using AtlasOps.Features;

public sealed class CommandTelemetryService(
    IAtlasOpsCapabilityRepository<CommandTelemetryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CommandTelemetryValidator validator = new();
    private readonly CommandTelemetryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CommandTelemetryChanged>> ExecuteAsync(
        UpdateCommandTelemetryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CommandTelemetryChanged>.Invalid(issues);
        }

        CommandTelemetryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CommandTelemetryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CommandTelemetryChanged>.Invalid(
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

        CommandTelemetryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CommandTelemetryChanged>.Success(changed);
    }
}