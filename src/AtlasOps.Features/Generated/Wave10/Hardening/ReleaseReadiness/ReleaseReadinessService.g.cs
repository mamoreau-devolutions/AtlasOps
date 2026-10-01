namespace AtlasOps.Features.Hardening.ReleaseReadiness;

using AtlasOps.Features;

public sealed class ReleaseReadinessService(
    IAtlasOpsCapabilityRepository<ReleaseReadinessItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseReadinessValidator validator = new();
    private readonly ReleaseReadinessPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseReadinessChanged>> ExecuteAsync(
        UpdateReleaseReadinessCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseReadinessChanged>.Invalid(issues);
        }

        ReleaseReadinessItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseReadinessItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseReadinessChanged>.Invalid(
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

        ReleaseReadinessChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseReadinessChanged>.Success(changed);
    }
}