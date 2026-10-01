namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementRevocationState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementRevocationRecord(Guid Id, string Name, string Owner, EntitlementRevocationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementRevocationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementRevocationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementRevocationQuery(string? SearchText, EntitlementRevocationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementRevocationPage(IReadOnlyList<EntitlementRevocationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementRevocationMutation(bool Succeeded, string Code, string Message, EntitlementRevocationRecord? Record, EntitlementRevocationEvent? Event);
public interface IEntitlementRevocationRepository
{
    ValueTask<EntitlementRevocationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementRevocationPage> QueryAsync(EntitlementRevocationQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementRevocationMutation> SaveAsync(EntitlementRevocationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementRevocationEventSink { ValueTask PublishAsync(EntitlementRevocationEvent domainEvent, CancellationToken cancellationToken); }