namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationReportingRecord(Guid Id, string Name, string Owner, ObligationReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationReportingQuery(string? SearchText, ObligationReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationReportingPage(IReadOnlyList<ObligationReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationReportingMutation(bool Succeeded, string Code, string Message, ObligationReportingRecord? Record, ObligationReportingEvent? Event);
public interface IObligationReportingRepository
{
    ValueTask<ObligationReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationReportingPage> QueryAsync(ObligationReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationReportingMutation> SaveAsync(ObligationReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationReportingEventSink { ValueTask PublishAsync(ObligationReportingEvent domainEvent, CancellationToken cancellationToken); }