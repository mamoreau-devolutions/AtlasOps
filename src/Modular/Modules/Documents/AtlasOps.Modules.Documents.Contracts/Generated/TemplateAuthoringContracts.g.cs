namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TemplateAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record TemplateAuthoringRecord(Guid Id, string Name, string Owner, TemplateAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TemplateAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TemplateAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TemplateAuthoringQuery(string? SearchText, TemplateAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TemplateAuthoringPage(IReadOnlyList<TemplateAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TemplateAuthoringMutation(bool Succeeded, string Code, string Message, TemplateAuthoringRecord? Record, TemplateAuthoringEvent? Event);
public interface ITemplateAuthoringRepository
{
    ValueTask<TemplateAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TemplateAuthoringPage> QueryAsync(TemplateAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<TemplateAuthoringMutation> SaveAsync(TemplateAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITemplateAuthoringEventSink { ValueTask PublishAsync(TemplateAuthoringEvent domainEvent, CancellationToken cancellationToken); }