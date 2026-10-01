namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementProvisioningRecord(Guid Id, string Name, string Owner, EntitlementProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementProvisioningQuery(string? SearchText, EntitlementProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementProvisioningPage(IReadOnlyList<EntitlementProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementProvisioningMutation(bool Succeeded, string Code, string Message, EntitlementProvisioningRecord? Record, EntitlementProvisioningEvent? Event);
public interface IEntitlementProvisioningRepository
{
    ValueTask<EntitlementProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementProvisioningPage> QueryAsync(EntitlementProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementProvisioningMutation> SaveAsync(EntitlementProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementProvisioningEventSink { ValueTask PublishAsync(EntitlementProvisioningEvent domainEvent, CancellationToken cancellationToken); }