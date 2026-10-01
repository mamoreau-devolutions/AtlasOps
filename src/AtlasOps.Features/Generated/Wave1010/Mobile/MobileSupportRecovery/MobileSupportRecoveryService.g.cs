namespace AtlasOps.Features.Mobile.MobileSupportRecovery;

using AtlasOps.Features;

public sealed class MobileSupportRecoveryService(
    IAtlasOpsCapabilityRepository<MobileSupportRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileSupportRecoveryValidator validator = new();
    private readonly MobileSupportRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileSupportRecoveryChanged>> ExecuteAsync(
        UpdateMobileSupportRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileSupportRecoveryChanged>.Invalid(issues);
        }

        MobileSupportRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileSupportRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileSupportRecoveryChanged>.Invalid(
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

        MobileSupportRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileSupportRecoveryChanged>.Success(changed);
    }
}