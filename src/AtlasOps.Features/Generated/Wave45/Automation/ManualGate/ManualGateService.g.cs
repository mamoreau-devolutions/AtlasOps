namespace AtlasOps.Features.Automation.ManualGate;

using AtlasOps.Features;

public sealed class ManualGateService(
    IAtlasOpsCapabilityRepository<ManualGateItem> repository,
    TimeProvider timeProvider)
{
    private readonly ManualGateValidator validator = new();
    private readonly ManualGatePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ManualGateChanged>> ExecuteAsync(
        UpdateManualGateCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ManualGateChanged>.Invalid(issues);
        }

        ManualGateItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ManualGateItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ManualGateChanged>.Invalid(
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

        ManualGateChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ManualGateChanged>.Success(changed);
    }
}