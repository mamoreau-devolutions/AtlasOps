namespace AtlasOps.Features.Mobile.MobileCertificateProvisioning;

using AtlasOps.Features;

public sealed class MobileCertificateProvisioningService(
    IAtlasOpsCapabilityRepository<MobileCertificateProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileCertificateProvisioningValidator validator = new();
    private readonly MobileCertificateProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileCertificateProvisioningChanged>> ExecuteAsync(
        UpdateMobileCertificateProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileCertificateProvisioningChanged>.Invalid(issues);
        }

        MobileCertificateProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileCertificateProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileCertificateProvisioningChanged>.Invalid(
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

        MobileCertificateProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileCertificateProvisioningChanged>.Success(changed);
    }
}