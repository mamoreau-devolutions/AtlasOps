namespace AtlasOps.Features.FinOps.FinOpsReportGovernance;

using AtlasOps.Features;

public sealed class FinOpsReportGovernanceService(
    IAtlasOpsCapabilityRepository<FinOpsReportGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly FinOpsReportGovernanceValidator validator = new();
    private readonly FinOpsReportGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<FinOpsReportGovernanceChanged>> ExecuteAsync(
        UpdateFinOpsReportGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FinOpsReportGovernanceChanged>.Invalid(issues);
        }

        FinOpsReportGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FinOpsReportGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FinOpsReportGovernanceChanged>.Invalid(
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

        FinOpsReportGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FinOpsReportGovernanceChanged>.Success(changed);
    }
}