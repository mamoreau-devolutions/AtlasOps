namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplateIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplateIndexingRecord(Guid Id, string Name, string Owner, TemplateIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplateIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplateIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplateIndexingQuery(string? SearchText, TemplateIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplateIndexingPage(IReadOnlyList<TemplateIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplateIndexingMutation(bool Succeeded, string Code, string Message, TemplateIndexingRecord? Record, TemplateIndexingEvent? Event);
public interface ITemplateIndexingRepository
{
    ValueTask<TemplateIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplateIndexingPage> QueryAsync(TemplateIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<TemplateIndexingMutation> SaveAsync(TemplateIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplateIndexingEventSink { ValueTask PublishAsync(TemplateIndexingEvent domainEvent, CancellationToken cancellationToken); }