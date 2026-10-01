namespace AtlasOps.Features.Hardening.StartupProbe;

using AtlasOps.Features;

public sealed class StartupProbeService(
    IAtlasOpsCapabilityRepository<StartupProbeItem> repository,
    TimeProvider timeProvider)
{
    private readonly StartupProbeValidator validator = new();
    private readonly StartupProbePolicy policy = new();

    public async Task<AtlasOpsOperationResult<StartupProbeChanged>> ExecuteAsync(
        UpdateStartupProbeCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<StartupProbeChanged>.Invalid(issues);
        }

        StartupProbeItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new StartupProbeItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<StartupProbeChanged>.Invalid(
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

        StartupProbeChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<StartupProbeChanged>.Success(changed);
    }
}