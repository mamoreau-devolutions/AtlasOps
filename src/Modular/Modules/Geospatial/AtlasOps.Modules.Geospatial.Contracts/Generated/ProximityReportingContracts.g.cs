namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityReportingRecord(Guid Id, string Name, string Owner, ProximityReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityReportingQuery(string? SearchText, ProximityReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityReportingPage(IReadOnlyList<ProximityReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityReportingMutation(bool Succeeded, string Code, string Message, ProximityReportingRecord? Record, ProximityReportingEvent? Event);
public interface IProximityReportingRepository
{
    ValueTask<ProximityReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityReportingPage> QueryAsync(ProximityReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityReportingMutation> SaveAsync(ProximityReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityReportingEventSink { ValueTask PublishAsync(ProximityReportingEvent domainEvent, CancellationToken cancellationToken); }