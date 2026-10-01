namespace AtlasOps.Features.Hardening.DiagnosticSnapshot;

using AtlasOps.Features;

public sealed class DiagnosticSnapshotService(
    IAtlasOpsCapabilityRepository<DiagnosticSnapshotItem> repository,
    TimeProvider timeProvider)
{
    private readonly DiagnosticSnapshotValidator validator = new();
    private readonly DiagnosticSnapshotPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DiagnosticSnapshotChanged>> ExecuteAsync(
        UpdateDiagnosticSnapshotCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DiagnosticSnapshotChanged>.Invalid(issues);
        }

        DiagnosticSnapshotItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DiagnosticSnapshotItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DiagnosticSnapshotChanged>.Invalid(
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

        DiagnosticSnapshotChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DiagnosticSnapshotChanged>.Success(changed);
    }
}