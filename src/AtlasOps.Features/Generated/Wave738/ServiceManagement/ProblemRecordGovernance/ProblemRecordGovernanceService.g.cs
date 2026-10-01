namespace AtlasOps.Features.ServiceManagement.ProblemRecordGovernance;

using AtlasOps.Features;

public sealed class ProblemRecordGovernanceService(
    IAtlasOpsCapabilityRepository<ProblemRecordGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ProblemRecordGovernanceValidator validator = new();
    private readonly ProblemRecordGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ProblemRecordGovernanceChanged>> ExecuteAsync(
        UpdateProblemRecordGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ProblemRecordGovernanceChanged>.Invalid(issues);
        }

        ProblemRecordGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ProblemRecordGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ProblemRecordGovernanceChanged>.Invalid(
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

        ProblemRecordGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ProblemRecordGovernanceChanged>.Success(changed);
    }
}