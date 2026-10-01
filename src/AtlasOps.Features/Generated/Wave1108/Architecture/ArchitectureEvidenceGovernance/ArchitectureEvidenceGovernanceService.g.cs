namespace AtlasOps.Features.Architecture.ArchitectureEvidenceGovernance;

using AtlasOps.Features;

public sealed class ArchitectureEvidenceGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureEvidenceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureEvidenceGovernanceValidator validator = new();
    private readonly ArchitectureEvidenceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureEvidenceGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureEvidenceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceGovernanceChanged>.Invalid(issues);
        }

        ArchitectureEvidenceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureEvidenceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceGovernanceChanged>.Invalid(
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

        ArchitectureEvidenceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureEvidenceGovernanceChanged>.Success(changed);
    }
}