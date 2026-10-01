namespace AtlasOps.Features.Architecture.ArchitectureEvidenceRecovery;

using AtlasOps.Features;

public sealed class ArchitectureEvidenceRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureEvidenceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureEvidenceRecoveryValidator validator = new();
    private readonly ArchitectureEvidenceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureEvidenceRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureEvidenceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceRecoveryChanged>.Invalid(issues);
        }

        ArchitectureEvidenceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureEvidenceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureEvidenceRecoveryChanged>.Invalid(
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

        ArchitectureEvidenceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureEvidenceRecoveryChanged>.Success(changed);
    }
}