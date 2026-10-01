namespace AtlasOps.Features.Hardening.UpgradeAssessment;

using AtlasOps.Features;

public sealed class UpgradeAssessmentService(
    IAtlasOpsCapabilityRepository<UpgradeAssessmentItem> repository,
    TimeProvider timeProvider)
{
    private readonly UpgradeAssessmentValidator validator = new();
    private readonly UpgradeAssessmentPolicy policy = new();

    public async Task<AtlasOpsOperationResult<UpgradeAssessmentChanged>> ExecuteAsync(
        UpdateUpgradeAssessmentCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<UpgradeAssessmentChanged>.Invalid(issues);
        }

        UpgradeAssessmentItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new UpgradeAssessmentItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<UpgradeAssessmentChanged>.Invalid(
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

        UpgradeAssessmentChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<UpgradeAssessmentChanged>.Success(changed);
    }
}