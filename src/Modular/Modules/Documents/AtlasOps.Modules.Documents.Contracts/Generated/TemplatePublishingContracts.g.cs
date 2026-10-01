namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplatePublishingState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplatePublishingRecord(Guid Id, string Name, string Owner, TemplatePublishingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplatePublishingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplatePublishingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplatePublishingQuery(string? SearchText, TemplatePublishingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplatePublishingPage(IReadOnlyList<TemplatePublishingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplatePublishingMutation(bool Succeeded, string Code, string Message, TemplatePublishingRecord? Record, TemplatePublishingEvent? Event);
public interface ITemplatePublishingRepository
{
    ValueTask<TemplatePublishingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplatePublishingPage> QueryAsync(TemplatePublishingQuery query, CancellationToken cancellationToken);
    ValueTask<TemplatePublishingMutation> SaveAsync(TemplatePublishingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplatePublishingEventSink { ValueTask PublishAsync(TemplatePublishingEvent domainEvent, CancellationToken cancellationToken); }