namespace AtlasOps.Features.Hardening.ReleaseEvidence;

using AtlasOps.Features;

public sealed class ReleaseEvidenceService(
    IAtlasOpsCapabilityRepository<ReleaseEvidenceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseEvidenceValidator validator = new();
    private readonly ReleaseEvidencePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseEvidenceChanged>> ExecuteAsync(
        UpdateReleaseEvidenceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseEvidenceChanged>.Invalid(issues);
        }

        ReleaseEvidenceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseEvidenceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseEvidenceChanged>.Invalid(
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

        ReleaseEvidenceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseEvidenceChanged>.Success(changed);
    }
}