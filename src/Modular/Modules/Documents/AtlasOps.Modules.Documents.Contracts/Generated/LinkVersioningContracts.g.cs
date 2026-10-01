namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkVersioningState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkVersioningRecord(Guid Id, string Name, string Owner, LinkVersioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkVersioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkVersioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkVersioningQuery(string? SearchText, LinkVersioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkVersioningPage(IReadOnlyList<LinkVersioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkVersioningMutation(bool Succeeded, string Code, string Message, LinkVersioningRecord? Record, LinkVersioningEvent? Event);
public interface ILinkVersioningRepository
{
    ValueTask<LinkVersioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkVersioningPage> QueryAsync(LinkVersioningQuery query, CancellationToken cancellationToken);
    ValueTask<LinkVersioningMutation> SaveAsync(LinkVersioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkVersioningEventSink { ValueTask PublishAsync(LinkVersioningEvent domainEvent, CancellationToken cancellationToken); }