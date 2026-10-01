namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkAuthoringRecord(Guid Id, string Name, string Owner, LinkAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkAuthoringQuery(string? SearchText, LinkAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkAuthoringPage(IReadOnlyList<LinkAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkAuthoringMutation(bool Succeeded, string Code, string Message, LinkAuthoringRecord? Record, LinkAuthoringEvent? Event);
public interface ILinkAuthoringRepository
{
    ValueTask<LinkAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkAuthoringPage> QueryAsync(LinkAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<LinkAuthoringMutation> SaveAsync(LinkAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkAuthoringEventSink { ValueTask PublishAsync(LinkAuthoringEvent domainEvent, CancellationToken cancellationToken); }