namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record PortReportingRecord(Guid Id, string Name, string Owner, PortReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortReportingQuery(string? SearchText, PortReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortReportingPage(IReadOnlyList<PortReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortReportingMutation(bool Succeeded, string Code, string Message, PortReportingRecord? Record, PortReportingEvent? Event);
public interface IPortReportingRepository
{
    ValueTask<PortReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortReportingPage> QueryAsync(PortReportingQuery query, CancellationToken cancellationToken);
    ValueTask<PortReportingMutation> SaveAsync(PortReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortReportingEventSink { ValueTask PublishAsync(PortReportingEvent domainEvent, CancellationToken cancellationToken); }