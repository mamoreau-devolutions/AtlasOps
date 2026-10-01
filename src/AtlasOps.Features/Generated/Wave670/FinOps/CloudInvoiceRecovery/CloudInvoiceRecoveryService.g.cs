namespace AtlasOps.Features.FinOps.CloudInvoiceRecovery;

using AtlasOps.Features;

public sealed class CloudInvoiceRecoveryService(
    IAtlasOpsCapabilityRepository<CloudInvoiceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudInvoiceRecoveryValidator validator = new();
    private readonly CloudInvoiceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudInvoiceRecoveryChanged>> ExecuteAsync(
        UpdateCloudInvoiceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudInvoiceRecoveryChanged>.Invalid(issues);
        }

        CloudInvoiceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudInvoiceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudInvoiceRecoveryChanged>.Invalid(
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

        CloudInvoiceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudInvoiceRecoveryChanged>.Success(changed);
    }
}