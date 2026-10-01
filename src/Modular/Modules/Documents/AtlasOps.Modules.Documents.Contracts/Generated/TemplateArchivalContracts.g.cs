namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplateArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplateArchivalRecord(Guid Id, string Name, string Owner, TemplateArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplateArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplateArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplateArchivalQuery(string? SearchText, TemplateArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplateArchivalPage(IReadOnlyList<TemplateArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplateArchivalMutation(bool Succeeded, string Code, string Message, TemplateArchivalRecord? Record, TemplateArchivalEvent? Event);
public interface ITemplateArchivalRepository
{
    ValueTask<TemplateArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplateArchivalPage> QueryAsync(TemplateArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<TemplateArchivalMutation> SaveAsync(TemplateArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplateArchivalEventSink { ValueTask PublishAsync(TemplateArchivalEvent domainEvent, CancellationToken cancellationToken); }