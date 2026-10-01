namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkArchivalRecord(Guid Id, string Name, string Owner, LinkArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkArchivalQuery(string? SearchText, LinkArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkArchivalPage(IReadOnlyList<LinkArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkArchivalMutation(bool Succeeded, string Code, string Message, LinkArchivalRecord? Record, LinkArchivalEvent? Event);
public interface ILinkArchivalRepository
{
    ValueTask<LinkArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkArchivalPage> QueryAsync(LinkArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<LinkArchivalMutation> SaveAsync(LinkArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkArchivalEventSink { ValueTask PublishAsync(LinkArchivalEvent domainEvent, CancellationToken cancellationToken); }