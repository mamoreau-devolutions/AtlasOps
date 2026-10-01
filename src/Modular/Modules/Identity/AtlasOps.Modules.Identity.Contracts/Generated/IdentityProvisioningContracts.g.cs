namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityProvisioningRecord(Guid Id, string Name, string Owner, IdentityProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityProvisioningQuery(string? SearchText, IdentityProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityProvisioningPage(IReadOnlyList<IdentityProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityProvisioningMutation(bool Succeeded, string Code, string Message, IdentityProvisioningRecord? Record, IdentityProvisioningEvent? Event);
public interface IIdentityProvisioningRepository
{
    ValueTask<IdentityProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityProvisioningPage> QueryAsync(IdentityProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityProvisioningMutation> SaveAsync(IdentityProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityProvisioningEventSink { ValueTask PublishAsync(IdentityProvisioningEvent domainEvent, CancellationToken cancellationToken); }