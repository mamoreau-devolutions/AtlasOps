namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IncidentReviewState { Draft, Active, Paused, Completed, Archived }
public sealed record IncidentReviewRecord(Guid Id, string Name, string Owner, IncidentReviewState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IncidentReviewCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IncidentReviewEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IncidentReviewQuery(string? SearchText, IncidentReviewState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IncidentReviewPage(IReadOnlyList<IncidentReviewRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IncidentReviewMutation(bool Succeeded, string Code, string Message, IncidentReviewRecord? Record, IncidentReviewEvent? Event);
public interface IIncidentReviewRepository
{
    ValueTask<IncidentReviewRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IncidentReviewPage> QueryAsync(IncidentReviewQuery query, CancellationToken cancellationToken);
    ValueTask<IncidentReviewMutation> SaveAsync(IncidentReviewRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIncidentReviewEventSink { ValueTask PublishAsync(IncidentReviewEvent domainEvent, CancellationToken cancellationToken); }