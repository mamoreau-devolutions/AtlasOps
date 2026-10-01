namespace AtlasOps.Features.Mobile.MobileCertificateGovernance;

using AtlasOps.Features;

public sealed class MobileCertificateGovernanceService(
    IAtlasOpsCapabilityRepository<MobileCertificateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileCertificateGovernanceValidator validator = new();
    private readonly MobileCertificateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileCertificateGovernanceChanged>> ExecuteAsync(
        UpdateMobileCertificateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileCertificateGovernanceChanged>.Invalid(issues);
        }

        MobileCertificateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileCertificateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileCertificateGovernanceChanged>.Invalid(
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

        MobileCertificateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileCertificateGovernanceChanged>.Success(changed);
    }
}