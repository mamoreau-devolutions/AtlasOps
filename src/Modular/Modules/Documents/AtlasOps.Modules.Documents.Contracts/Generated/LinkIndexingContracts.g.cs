namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkIndexingRecord(Guid Id, string Name, string Owner, LinkIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkIndexingQuery(string? SearchText, LinkIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkIndexingPage(IReadOnlyList<LinkIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkIndexingMutation(bool Succeeded, string Code, string Message, LinkIndexingRecord? Record, LinkIndexingEvent? Event);
public interface ILinkIndexingRepository
{
    ValueTask<LinkIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkIndexingPage> QueryAsync(LinkIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<LinkIndexingMutation> SaveAsync(LinkIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkIndexingEventSink { ValueTask PublishAsync(LinkIndexingEvent domainEvent, CancellationToken cancellationToken); }