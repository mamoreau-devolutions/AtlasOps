namespace AtlasOps.Features.Mobile.MobileProfileRecovery;

using AtlasOps.Features;

public sealed class MobileProfileRecoveryService(
    IAtlasOpsCapabilityRepository<MobileProfileRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileProfileRecoveryValidator validator = new();
    private readonly MobileProfileRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileProfileRecoveryChanged>> ExecuteAsync(
        UpdateMobileProfileRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileProfileRecoveryChanged>.Invalid(issues);
        }

        MobileProfileRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileProfileRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileProfileRecoveryChanged>.Invalid(
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

        MobileProfileRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileProfileRecoveryChanged>.Success(changed);
    }
}