namespace AtlasOps.Features.Governance.ComplianceControl;

using AtlasOps.Features;

public sealed class ComplianceControlService(
    IAtlasOpsCapabilityRepository<ComplianceControlItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComplianceControlValidator validator = new();
    private readonly ComplianceControlPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComplianceControlChanged>> ExecuteAsync(
        UpdateComplianceControlCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComplianceControlChanged>.Invalid(issues);
        }

        ComplianceControlItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComplianceControlItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComplianceControlChanged>.Invalid(
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

        ComplianceControlChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComplianceControlChanged>.Success(changed);
    }
}