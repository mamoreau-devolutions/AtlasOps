namespace AtlasOps.Features.Architecture.ArchitectureDecisionRecovery;

using AtlasOps.Features;

public sealed class ArchitectureDecisionRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureDecisionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDecisionRecoveryValidator validator = new();
    private readonly ArchitectureDecisionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDecisionRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureDecisionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDecisionRecoveryChanged>.Invalid(issues);
        }

        ArchitectureDecisionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDecisionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDecisionRecoveryChanged>.Invalid(
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

        ArchitectureDecisionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDecisionRecoveryChanged>.Success(changed);
    }
}