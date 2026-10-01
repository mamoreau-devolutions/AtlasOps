namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureReportingRecord(Guid Id, string Name, string Owner, FeatureReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureReportingQuery(string? SearchText, FeatureReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureReportingPage(IReadOnlyList<FeatureReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureReportingMutation(bool Succeeded, string Code, string Message, FeatureReportingRecord? Record, FeatureReportingEvent? Event);
public interface IFeatureReportingRepository
{
    ValueTask<FeatureReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureReportingPage> QueryAsync(FeatureReportingQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureReportingMutation> SaveAsync(FeatureReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureReportingEventSink { ValueTask PublishAsync(FeatureReportingEvent domainEvent, CancellationToken cancellationToken); }