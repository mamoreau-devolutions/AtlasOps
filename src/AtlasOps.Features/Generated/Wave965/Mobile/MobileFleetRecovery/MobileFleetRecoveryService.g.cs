namespace AtlasOps.Features.Mobile.MobileFleetRecovery;

using AtlasOps.Features;

public sealed class MobileFleetRecoveryService(
    IAtlasOpsCapabilityRepository<MobileFleetRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileFleetRecoveryValidator validator = new();
    private readonly MobileFleetRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileFleetRecoveryChanged>> ExecuteAsync(
        UpdateMobileFleetRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileFleetRecoveryChanged>.Invalid(issues);
        }

        MobileFleetRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileFleetRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileFleetRecoveryChanged>.Invalid(
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

        MobileFleetRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileFleetRecoveryChanged>.Success(changed);
    }
}