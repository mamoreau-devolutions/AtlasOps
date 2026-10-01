namespace AtlasOps.Features.Security.SecurityCertificateOptimization;

using AtlasOps.Features;

public sealed class SecurityCertificateOptimizationService(
    IAtlasOpsCapabilityRepository<SecurityCertificateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityCertificateOptimizationValidator validator = new();
    private readonly SecurityCertificateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityCertificateOptimizationChanged>> ExecuteAsync(
        UpdateSecurityCertificateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityCertificateOptimizationChanged>.Invalid(issues);
        }

        SecurityCertificateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityCertificateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityCertificateOptimizationChanged>.Invalid(
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

        SecurityCertificateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityCertificateOptimizationChanged>.Success(changed);
    }
}