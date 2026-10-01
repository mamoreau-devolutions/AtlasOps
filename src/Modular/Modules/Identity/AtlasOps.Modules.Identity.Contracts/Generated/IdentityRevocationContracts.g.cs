namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityRevocationState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityRevocationRecord(Guid Id, string Name, string Owner, IdentityRevocationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityRevocationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityRevocationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityRevocationQuery(string? SearchText, IdentityRevocationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityRevocationPage(IReadOnlyList<IdentityRevocationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityRevocationMutation(bool Succeeded, string Code, string Message, IdentityRevocationRecord? Record, IdentityRevocationEvent? Event);
public interface IIdentityRevocationRepository
{
    ValueTask<IdentityRevocationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityRevocationPage> QueryAsync(IdentityRevocationQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityRevocationMutation> SaveAsync(IdentityRevocationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityRevocationEventSink { ValueTask PublishAsync(IdentityRevocationEvent domainEvent, CancellationToken cancellationToken); }