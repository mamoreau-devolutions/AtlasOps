namespace AtlasOps.Features.Mobile.MobileCertificateRecovery;

using AtlasOps.Features;

public sealed class MobileCertificateRecoveryService(
    IAtlasOpsCapabilityRepository<MobileCertificateRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileCertificateRecoveryValidator validator = new();
    private readonly MobileCertificateRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileCertificateRecoveryChanged>> ExecuteAsync(
        UpdateMobileCertificateRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileCertificateRecoveryChanged>.Invalid(issues);
        }

        MobileCertificateRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileCertificateRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileCertificateRecoveryChanged>.Invalid(
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

        MobileCertificateRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileCertificateRecoveryChanged>.Success(changed);
    }
}