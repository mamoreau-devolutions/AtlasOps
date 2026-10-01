namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionArchivalRecord(Guid Id, string Name, string Owner, RevisionArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionArchivalQuery(string? SearchText, RevisionArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionArchivalPage(IReadOnlyList<RevisionArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionArchivalMutation(bool Succeeded, string Code, string Message, RevisionArchivalRecord? Record, RevisionArchivalEvent? Event);
public interface IRevisionArchivalRepository
{
    ValueTask<RevisionArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionArchivalPage> QueryAsync(RevisionArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionArchivalMutation> SaveAsync(RevisionArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionArchivalEventSink { ValueTask PublishAsync(RevisionArchivalEvent domainEvent, CancellationToken cancellationToken); }