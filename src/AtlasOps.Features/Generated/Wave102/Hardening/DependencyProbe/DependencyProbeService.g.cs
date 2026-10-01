namespace AtlasOps.Features.Hardening.DependencyProbe;

using AtlasOps.Features;

public sealed class DependencyProbeService(
    IAtlasOpsCapabilityRepository<DependencyProbeItem> repository,
    TimeProvider timeProvider)
{
    private readonly DependencyProbeValidator validator = new();
    private readonly DependencyProbePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DependencyProbeChanged>> ExecuteAsync(
        UpdateDependencyProbeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DependencyProbeChanged>.Invalid(issues);
        }

        DependencyProbeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DependencyProbeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DependencyProbeChanged>.Invalid(
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

        DependencyProbeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DependencyProbeChanged>.Success(changed);
    }
}