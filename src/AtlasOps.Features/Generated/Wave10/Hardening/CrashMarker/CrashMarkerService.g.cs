namespace AtlasOps.Features.Hardening.CrashMarker;

using AtlasOps.Features;

public sealed class CrashMarkerService(
    IAtlasOpsCapabilityRepository<CrashMarkerItem> repository,
    TimeProvider timeProvider)
{
    private readonly CrashMarkerValidator validator = new();
    private readonly CrashMarkerPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CrashMarkerChanged>> ExecuteAsync(
        UpdateCrashMarkerCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CrashMarkerChanged>.Invalid(issues);
        }

        CrashMarkerItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CrashMarkerItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CrashMarkerChanged>.Invalid(
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

        CrashMarkerChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CrashMarkerChanged>.Success(changed);
    }
}