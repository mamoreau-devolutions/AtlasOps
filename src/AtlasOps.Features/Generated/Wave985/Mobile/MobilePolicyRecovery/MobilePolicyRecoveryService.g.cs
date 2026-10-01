namespace AtlasOps.Features.Mobile.MobilePolicyRecovery;

using AtlasOps.Features;

public sealed class MobilePolicyRecoveryService(
    IAtlasOpsCapabilityRepository<MobilePolicyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobilePolicyRecoveryValidator validator = new();
    private readonly MobilePolicyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobilePolicyRecoveryChanged>> ExecuteAsync(
        UpdateMobilePolicyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobilePolicyRecoveryChanged>.Invalid(issues);
        }

        MobilePolicyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobilePolicyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobilePolicyRecoveryChanged>.Invalid(
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

        MobilePolicyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobilePolicyRecoveryChanged>.Success(changed);
    }
}