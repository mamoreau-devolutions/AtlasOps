namespace AtlasOps.Features.Connections.CertificateTrust;

using AtlasOps.Features;

public sealed class CertificateTrustService(
    IAtlasOpsCapabilityRepository<CertificateTrustItem> repository,
    TimeProvider timeProvider)
{
    private readonly CertificateTrustValidator validator = new();
    private readonly CertificateTrustPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CertificateTrustChanged>> ExecuteAsync(
        UpdateCertificateTrustCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CertificateTrustChanged>.Invalid(issues);
        }

        CertificateTrustItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CertificateTrustItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CertificateTrustChanged>.Invalid(
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

        CertificateTrustChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CertificateTrustChanged>.Success(changed);
    }
}