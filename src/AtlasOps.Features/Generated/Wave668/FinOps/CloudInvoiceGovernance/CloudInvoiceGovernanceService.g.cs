namespace AtlasOps.Features.FinOps.CloudInvoiceGovernance;

using AtlasOps.Features;

public sealed class CloudInvoiceGovernanceService(
    IAtlasOpsCapabilityRepository<CloudInvoiceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudInvoiceGovernanceValidator validator = new();
    private readonly CloudInvoiceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudInvoiceGovernanceChanged>> ExecuteAsync(
        UpdateCloudInvoiceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudInvoiceGovernanceChanged>.Invalid(issues);
        }

        CloudInvoiceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudInvoiceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudInvoiceGovernanceChanged>.Invalid(
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

        CloudInvoiceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudInvoiceGovernanceChanged>.Success(changed);
    }
}