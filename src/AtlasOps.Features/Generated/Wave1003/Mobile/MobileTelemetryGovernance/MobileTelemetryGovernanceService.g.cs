namespace AtlasOps.Features.Mobile.MobileTelemetryGovernance;

using AtlasOps.Features;

public sealed class MobileTelemetryGovernanceService(
    IAtlasOpsCapabilityRepository<MobileTelemetryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileTelemetryGovernanceValidator validator = new();
    private readonly MobileTelemetryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileTelemetryGovernanceChanged>> ExecuteAsync(
        UpdateMobileTelemetryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileTelemetryGovernanceChanged>.Invalid(issues);
        }

        MobileTelemetryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileTelemetryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileTelemetryGovernanceChanged>.Invalid(
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

        MobileTelemetryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileTelemetryGovernanceChanged>.Success(changed);
    }
}