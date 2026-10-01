namespace AtlasOps.Features.Hardening.ResponsivenessProbe;

using AtlasOps.Features;

public sealed class ResponsivenessProbeService(
    IAtlasOpsCapabilityRepository<ResponsivenessProbeItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResponsivenessProbeValidator validator = new();
    private readonly ResponsivenessProbePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResponsivenessProbeChanged>> ExecuteAsync(
        UpdateResponsivenessProbeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResponsivenessProbeChanged>.Invalid(issues);
        }

        ResponsivenessProbeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResponsivenessProbeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResponsivenessProbeChanged>.Invalid(
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

        ResponsivenessProbeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResponsivenessProbeChanged>.Success(changed);
    }
}