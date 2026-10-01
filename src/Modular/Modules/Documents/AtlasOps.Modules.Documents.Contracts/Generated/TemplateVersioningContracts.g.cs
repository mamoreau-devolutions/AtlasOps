namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplateVersioningState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplateVersioningRecord(Guid Id, string Name, string Owner, TemplateVersioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplateVersioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplateVersioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplateVersioningQuery(string? SearchText, TemplateVersioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplateVersioningPage(IReadOnlyList<TemplateVersioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplateVersioningMutation(bool Succeeded, string Code, string Message, TemplateVersioningRecord? Record, TemplateVersioningEvent? Event);
public interface ITemplateVersioningRepository
{
    ValueTask<TemplateVersioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplateVersioningPage> QueryAsync(TemplateVersioningQuery query, CancellationToken cancellationToken);
    ValueTask<TemplateVersioningMutation> SaveAsync(TemplateVersioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplateVersioningEventSink { ValueTask PublishAsync(TemplateVersioningEvent domainEvent, CancellationToken cancellationToken); }