namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolReportingRecord(Guid Id, string Name, string Owner, ProtocolReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolReportingQuery(string? SearchText, ProtocolReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolReportingPage(IReadOnlyList<ProtocolReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolReportingMutation(bool Succeeded, string Code, string Message, ProtocolReportingRecord? Record, ProtocolReportingEvent? Event);
public interface IProtocolReportingRepository
{
    ValueTask<ProtocolReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolReportingPage> QueryAsync(ProtocolReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolReportingMutation> SaveAsync(ProtocolReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolReportingEventSink { ValueTask PublishAsync(ProtocolReportingEvent domainEvent, CancellationToken cancellationToken); }