namespace AtlasOps.Features.Automation.AutomationCredential;

using AtlasOps.Features;

public sealed class AutomationCredentialService(
    IAtlasOpsCapabilityRepository<AutomationCredentialItem> repository,
    TimeProvider timeProvider)
{
    private readonly AutomationCredentialValidator validator = new();
    private readonly AutomationCredentialPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AutomationCredentialChanged>> ExecuteAsync(
        UpdateAutomationCredentialCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AutomationCredentialChanged>.Invalid(issues);
        }

        AutomationCredentialItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AutomationCredentialItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AutomationCredentialChanged>.Invalid(
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

        AutomationCredentialChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AutomationCredentialChanged>.Success(changed);
    }
}