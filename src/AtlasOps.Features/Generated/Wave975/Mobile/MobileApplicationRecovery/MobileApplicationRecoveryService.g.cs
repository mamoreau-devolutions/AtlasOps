namespace AtlasOps.Features.Mobile.MobileApplicationRecovery;

using AtlasOps.Features;

public sealed class MobileApplicationRecoveryService(
    IAtlasOpsCapabilityRepository<MobileApplicationRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileApplicationRecoveryValidator validator = new();
    private readonly MobileApplicationRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileApplicationRecoveryChanged>> ExecuteAsync(
        UpdateMobileApplicationRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileApplicationRecoveryChanged>.Invalid(issues);
        }

        MobileApplicationRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileApplicationRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileApplicationRecoveryChanged>.Invalid(
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

        MobileApplicationRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileApplicationRecoveryChanged>.Success(changed);
    }
}