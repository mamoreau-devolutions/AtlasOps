namespace AtlasOps.Features.Automation.ApprovalRequest;

using AtlasOps.Features;

public sealed class ApprovalRequestService(
    IAtlasOpsCapabilityRepository<ApprovalRequestItem> repository,
    TimeProvider timeProvider)
{
    private readonly ApprovalRequestValidator validator = new();
    private readonly ApprovalRequestPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ApprovalRequestChanged>> ExecuteAsync(
        UpdateApprovalRequestCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ApprovalRequestChanged>.Invalid(issues);
        }

        ApprovalRequestItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ApprovalRequestItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ApprovalRequestChanged>.Invalid(
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

        ApprovalRequestChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ApprovalRequestChanged>.Success(changed);
    }
}