namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerReportingRecord(Guid Id, string Name, string Owner, LayerReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerReportingQuery(string? SearchText, LayerReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerReportingPage(IReadOnlyList<LayerReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerReportingMutation(bool Succeeded, string Code, string Message, LayerReportingRecord? Record, LayerReportingEvent? Event);
public interface ILayerReportingRepository
{
    ValueTask<LayerReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerReportingPage> QueryAsync(LayerReportingQuery query, CancellationToken cancellationToken);
    ValueTask<LayerReportingMutation> SaveAsync(LayerReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerReportingEventSink { ValueTask PublishAsync(LayerReportingEvent domainEvent, CancellationToken cancellationToken); }