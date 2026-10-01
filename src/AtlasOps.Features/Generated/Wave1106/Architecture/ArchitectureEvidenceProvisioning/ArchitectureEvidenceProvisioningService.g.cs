namespace AtlasOps.Features.Architecture.ArchitectureEvidenceProvisioning;

using AtlasOps.Features;

public sealed class ArchitectureEvidenceProvisioningService(
    IAtlasOpsCapabilityRepository<ArchitectureEvidenceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureEvidenceProvisioningValidator validator = new();
    private readonly ArchitectureEvidenceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureEvidenceProvisioningChanged>> ExecuteAsync(
        UpdateArchitectureEvidenceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceProvisioningChanged>.Invalid(issues);
        }

        ArchitectureEvidenceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureEvidenceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceProvisioningChanged>.Invalid(
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

        ArchitectureEvidenceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureEvidenceProvisioningChanged>.Success(changed);
    }
}