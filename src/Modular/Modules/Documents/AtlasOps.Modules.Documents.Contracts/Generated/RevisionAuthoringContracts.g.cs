namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionAuthoringRecord(Guid Id, string Name, string Owner, RevisionAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionAuthoringQuery(string? SearchText, RevisionAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionAuthoringPage(IReadOnlyList<RevisionAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionAuthoringMutation(bool Succeeded, string Code, string Message, RevisionAuthoringRecord? Record, RevisionAuthoringEvent? Event);
public interface IRevisionAuthoringRepository
{
    ValueTask<RevisionAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionAuthoringPage> QueryAsync(RevisionAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionAuthoringMutation> SaveAsync(RevisionAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionAuthoringEventSink { ValueTask PublishAsync(RevisionAuthoringEvent domainEvent, CancellationToken cancellationToken); }